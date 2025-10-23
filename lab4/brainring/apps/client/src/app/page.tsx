"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import styles from "./page.module.scss";

export default function Page() {
  const [name, setName] = useState("");
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    fetch("http://localhost:5000/api/users/auth/check", {
      credentials: "include",
    })
      .then(async (res) => {
        if (res.ok) router.push("/home");
      })
      .finally(() => setLoading(false));
  }, [router]);

  const handleAuth = async (type: "login" | "register") => {
    if (!name.trim()) return alert("Введите имя");

    const res = await fetch(`http://localhost:5000/api/users/${type}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ name }),
    });

    if (res.ok) {
      router.push("/home");
    } else {
      const data = await res.json().catch(() => ({}));
      alert(data.message || "Ошибка авторизации");
    }
  };

  if (loading) return <p className={styles.text}>Загрузка...</p>;

  return (
    <main className={styles.container}>
      <h1 className={styles.title}>BrainRing</h1>
      <input
        type="text"
        placeholder="Введите имя"
        value={name}
        onChange={(e) => setName(e.target.value)}
        className={styles.input}
      />
      <div className={styles.buttons}>
        <button onClick={() => handleAuth("login")}>Войти</button>
        <button onClick={() => handleAuth("register")}>Регистрация</button>
      </div>
    </main>
  );
}
