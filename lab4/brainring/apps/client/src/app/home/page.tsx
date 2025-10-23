"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import styles from "./home.module.scss";

export default function HomePage() {
  const [user, setUser] = useState<{ id: string; name: string } | null>(null);
  const [joinId, setJoinId] = useState("");
  const router = useRouter();

  useEffect(() => {
    fetch("http://localhost:5000/api/users/auth/check", {
      credentials: "include",
    }).then(async (res) => {
      if (!res.ok) return router.push("/");
      const data = await res.json();
      setUser(data);
    });
  }, [router]);

  if (!user) return <p className={styles.text}>Загрузка...</p>;

  const createRoom = async () => {
    const res = await fetch("http://localhost:5000/api/game/create", {
      method: "POST",
      credentials: "include",
    });
    if (!res.ok) return alert("Не удалось создать комнату");

    const data = await res.json();
    router.push(`/game/host/${data.id}`);
  };

  const joinRoom = async () => {
    if (!joinId.trim()) return alert("Введите ID комнаты");

    const res = await fetch(
      `http://localhost:5000/api/game/check?sessionId=${joinId}`,
      {
        credentials: "include",
      }
    );

    if (!res.ok) return alert("Комната не найдена или уже начата");

    router.push(`/game/${joinId}`);
  };

  return (
    <main className={styles.container}>
      <h1 className={styles.title}>Привет, {user.name} 👋</h1>
      <div className={styles.buttons}>
        <button onClick={createRoom}>Создать комнату</button>
        <input
          type="text"
          placeholder="Введите ID комнаты"
          value={joinId}
          onChange={(e) => setJoinId(e.target.value)}
          className={styles.input}
        />
        <button onClick={joinRoom}>Присоединиться</button>
      </div>
    </main>
  );
}
