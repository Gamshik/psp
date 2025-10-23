"use client";

import Image from "next/image";
import styles from "./page.module.css";
import { useEffect, useState } from "react";

let socket: WebSocket | null = null;

export function connectWebSocket() {
  socket = new WebSocket("ws://localhost:5000/ws");

  socket.onopen = () => console.log("✅ Connected to server");
  socket.onclose = () => console.log("❌ Disconnected");
  socket.onmessage = (event) => console.log("📩", event.data);
  socket.onerror = (err) => console.error("⚠️ Error:", err);
}

export function sendMessage(msg: string) {
  if (socket && socket.readyState === WebSocket.OPEN) {
    socket.send(msg);
  }
}

export default function Home() {
  const [msg, setMsg] = useState("");

  useEffect(() => {
    connectWebSocket();
  }, []);

  return (
    <div className={styles.page}>
      <main className={styles.main}>
        <div style={{ padding: 20 }}>
          <h1>BrainRing WebSocket Test</h1>
          <input value={msg} onChange={(e) => setMsg(e.target.value)} />
          <button onClick={() => sendMessage(msg)}>Send</button>
        </div>
        <Image
          className={styles.logo}
          src="/next.svg"
          alt="Next.js logo"
          width={180}
          height={38}
          priority
        />
        <ol>
          <li>
            Get started by editing <code>src/app/page.tsx</code>.
          </li>
          <li>Save and see your changes instantly.</li>
        </ol>

        <div className={styles.ctas}>
          <a
            className={styles.primary}
            href="https://vercel.com/new?utm_source=create-next-app&utm_medium=appdir-template&utm_campaign=create-next-app"
            target="_blank"
            rel="noopener noreferrer"
          >
            <Image
              className={styles.logo}
              src="/vercel.svg"
              alt="Vercel logomark"
              width={20}
              height={20}
            />
            Deploy now
          </a>
          <a
            href="https://nextjs.org/docs?utm_source=create-next-app&utm_medium=appdir-template&utm_campaign=create-next-app"
            target="_blank"
            rel="noopener noreferrer"
            className={styles.secondary}
          >
            Read our docs
          </a>
        </div>
      </main>
      <footer className={styles.footer}>
        <a
          href="https://nextjs.org/learn?utm_source=create-next-app&utm_medium=appdir-template&utm_campaign=create-next-app"
          target="_blank"
          rel="noopener noreferrer"
        >
          <Image
            aria-hidden
            src="/file.svg"
            alt="File icon"
            width={16}
            height={16}
          />
          Learn
        </a>
        <a
          href="https://vercel.com/templates?framework=next.js&utm_source=create-next-app&utm_medium=appdir-template&utm_campaign=create-next-app"
          target="_blank"
          rel="noopener noreferrer"
        >
          <Image
            aria-hidden
            src="/window.svg"
            alt="Window icon"
            width={16}
            height={16}
          />
          Examples
        </a>
        <a
          href="https://nextjs.org?utm_source=create-next-app&utm_medium=appdir-template&utm_campaign=create-next-app"
          target="_blank"
          rel="noopener noreferrer"
        >
          <Image
            aria-hidden
            src="/globe.svg"
            alt="Globe icon"
            width={16}
            height={16}
          />
          Go to nextjs.org →
        </a>
      </footer>
    </div>
  );
}
