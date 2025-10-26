"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import styles from "../../game.module.scss";
import { MessageTypes } from "../../enums";

interface Participant {
  UserId: string;
  Name: string;
  Score: number;
}

interface WsMessage {
  Type: MessageTypes;
  Payload: any;
}

export default function HostPage() {
  const params = useParams();
  const sessionId = params.id;
  const [participants, setParticipants] = useState<Participant[]>([]);
  const [questionText, setQuestionText] = useState("");
  const [options, setOptions] = useState(["", "", "", ""]);
  const [correctIndex, setCorrectIndex] = useState<number>(0);
  const [ws, setWs] = useState<WebSocket | null>(null);

  const router = useRouter();

  useEffect(() => {
    if (!sessionId) return;

    const socket = new WebSocket(
      `ws://localhost:5000/ws?sessionId=${sessionId}&isHost=true`
    );
    socket.onopen = () => console.log("✅ Connected as host");

    socket.onmessage = (event) => {
      const msg: WsMessage = JSON.parse(event.data);

      if (msg.Type === MessageTypes.UpdateParticipant) {
        setParticipants(msg.Payload.Participants);
      } else if (msg.Type === MessageTypes.AnswerResult) {
        const updatedParticipants: Participant[] = msg.Payload.Participants;
        setParticipants(updatedParticipants);
      }
    };

    socket.onclose = (ev) => {
      router.push(`/`);
    };

    setWs(socket);
    return () => socket.close();
  }, [sessionId]);

  const sendQuestion = () => {
    if (!ws) return;
    ws.send(
      JSON.stringify({
        Type: MessageTypes.NewQuestion,
        Payload: {
          Text: questionText,
          Options: options,
          CorrectOptionIndex: correctIndex,
        },
      })
    );

    setQuestionText("");
    setOptions(["", "", "", ""]);
    setCorrectIndex(0);
  };

  return (
    <div className={styles.container}>
      <h1>Хост сессии {sessionId}</h1>

      <div className={styles.section}>
        <h2>Участники</h2>
        <ul className={styles.participants_list}>
          {participants.map((p) => (
            <li key={p.UserId}>
              {p.Name} — очки: {p.Score}{" "}
            </li>
          ))}
        </ul>
      </div>

      <div className={styles.section}>
        <h2>Новый вопрос</h2>
        <div className={styles["input-group"]}>
          <input
            value={questionText}
            onChange={(e) => setQuestionText(e.target.value)}
            placeholder="Вопрос"
          />
          {options.map((opt, idx) => (
            <input
              key={idx}
              value={opt}
              onChange={(e) => {
                const newOpts = [...options];
                newOpts[idx] = e.target.value;
                setOptions(newOpts);
              }}
              placeholder={`Ответ ${idx + 1}`}
            />
          ))}
          <label>
            Правильный вариант:
            <select
              value={correctIndex}
              onChange={(e) => setCorrectIndex(Number(e.target.value))}
            >
              {options.map((_, idx) => (
                <option key={idx} value={idx}>
                  {idx + 1}
                </option>
              ))}
            </select>
          </label>
          <button onClick={sendQuestion}>Отправить вопрос</button>
        </div>
      </div>
    </div>
  );
}
