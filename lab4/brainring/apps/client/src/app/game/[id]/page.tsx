"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import styles from "../game.module.scss";
import { MessageTypes } from "../enums";

interface Participant {
  Id: string;
  Name: string;
  Score: number;
  Answered?: boolean;
}

interface Question {
  Text: string;
  Options: string[];
  CorrectOptionIndex?: number;
}

interface WsMessage {
  Type: MessageTypes;
  Payload: any;
}

export default function GamePage() {
  const params = useParams();
  const sessionId = params.id;
  const [participants, setParticipants] = useState<Participant[]>([]);
  const [question, setQuestion] = useState<Question | null>(null);
  const [answered, setAnswered] = useState(false);
  const [ws, setWs] = useState<WebSocket | null>(null);

  const router = useRouter();

  useEffect(() => {
    if (!sessionId) return;

    const socket = new WebSocket(
      `ws://localhost:5000/ws?sessionId=${sessionId}`
    );
    socket.onopen = () => console.log("✅ Connected as player");

    socket.onmessage = (event) => {
      const msg: WsMessage = JSON.parse(event.data);

      if (msg.Type === MessageTypes.NewQuestion) {
        setQuestion(msg.Payload);
        setAnswered(false);
      } else if (msg.Type === MessageTypes.AnswerResult) {
        setParticipants(msg.Payload.Participants);
      } else if (msg.Type === MessageTypes.UpdateParticipant) {
        setParticipants(msg.Payload.Participants);
      } else if (msg.Type === MessageTypes.CloseGame) {
        router.push(`/`);
      }
    };

    setWs(socket);
    return () => socket.close();
  }, [sessionId]);

  const sendAnswer = (optionIdx: number) => {
    if (!ws || answered) return;

    ws.send(
      JSON.stringify({
        Type: MessageTypes.AnswerResult,
        Payload: { SelectedOptionIndex: optionIdx },
      })
    );

    setAnswered(true);
  };

  return (
    <div className={styles.container}>
      <h1>Сессия {sessionId}</h1>

      <div className={styles.section}>
        <h2>Участники</h2>
        <ul className={styles.participants_list}>
          {participants.map((p) => (
            <li key={p.Id} className={p.Answered ? "answered" : ""}>
              {p.Name} — очки: {p.Score} {p.Answered ? "Ответил" : ""}
            </li>
          ))}
        </ul>
      </div>

      {question && (
        <div className={styles.section}>
          <h2>{question.Text}</h2>
          <ul>
            {question.Options.map((opt, idx) => (
              <li key={idx}>
                <button disabled={answered} onClick={() => sendAnswer(idx)}>
                  {opt}
                </button>
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
