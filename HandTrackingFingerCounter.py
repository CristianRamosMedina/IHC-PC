import cv2
import mediapipe as mp
import socket
import time

mp_hands = mp.solutions.hands
hands = mp_hands.Hands(static_image_mode=False, max_num_hands=1, min_detection_confidence=0.5)
mp_draw = mp.solutions.drawing_utils

while True:
    try:
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.connect(("localhost", 5050))
        print("¡Conectado a Unity!")
        break
    except ConnectionRefusedError:
        print("Esperando a que Unity abra el puerto 5050...")
        time.sleep(2)

cap = cv2.VideoCapture(0)
cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)

MAX_DIFF_PIX = 200
MAX_ANGLE = 45

while True:
    ret, img = cap.read()
    if not ret:
        break

    img_rgb = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
    results = hands.process(img_rgb)

    angle = 0

    if results.multi_hand_landmarks:
        handLms = results.multi_hand_landmarks[0]
        lmList = []
        h, w, c = img.shape

        for id, lm in enumerate(handLms.landmark):
            cx, cy = int(lm.x * w), int(lm.y * h)
            lmList.append((id, cx, cy))

        pulgar = next((x for x in lmList if x[0] == 4), None)
        menique = next((x for x in lmList if x[0] == 20), None)

        if pulgar and menique:
            diff = menique[2] - pulgar[2]
            angle = int(max(-MAX_ANGLE, min(MAX_ANGLE, (diff/MAX_DIFF_PIX)*MAX_ANGLE)))
            try:
                sock.sendall(f"{angle}\n".encode())
                print("Enviando a Unity:", angle)
            except Exception as e:
                print("Error enviando a Unity:", e)
                break

        mp_draw.draw_landmarks(img, handLms, mp_hands.HAND_CONNECTIONS)

    cv2.putText(img, f"{angle} grados", (30, 60), cv2.FONT_HERSHEY_SIMPLEX, 1.5, (0,255,0), 3)
    cv2.imshow("HandTracking", img)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

sock.close()
cap.release()
cv2.destroyAllWindows()
