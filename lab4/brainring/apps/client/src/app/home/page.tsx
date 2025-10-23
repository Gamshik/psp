"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import styles from "./home.module.scss";

interface User {
  id: string;
  name: string;
}

export default function HomePage() {
  const [user, setUser] = useState<User | null>(null);
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

  return (
    <main className={styles.container}>
      <h1 className={styles.title}>Привет, {user.name} 👋</h1>
      <div className={styles.buttons}>
        <button onClick={() => alert("Создание комнаты...")}>
          Создать комнату
        </button>
        <button onClick={() => alert("Присоединение к комнате...")}>
          Присоединиться
        </button>
      </div>
    </main>
  );
}
