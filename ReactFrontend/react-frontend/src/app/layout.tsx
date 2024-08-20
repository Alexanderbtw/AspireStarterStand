import { Metric } from "web-vitals";
import "./globals.css";
import Home from "./page";
import reportWebVitals from "./reportWebVitals";
import addTracing from "./observability/addTracing";

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  // addTracing(process.env.OTEL_EXPORTER_OTLP_PROTOCOL);
  // reportWebVitals(console.log);
  return (
    <html lang="en">
      <body>
        <Home
          weatherApi={process.env.services__apiservice__http__0 + "/weatherforecast"}
        />
      </body>
    </html>
  );
}