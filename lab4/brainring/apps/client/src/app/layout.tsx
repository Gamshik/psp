import "./globals.css";

export const metadata = {
  title: "BrainRing",
  description: "Simple game platform",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="ru">
      <body>{children}</body>
    </html>
  );
}
