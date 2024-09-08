import Image from "next/image";
import logo from "../../public/logo.svg";
import logoRepl from "../../public/Iam.png";
import Forecast from "./components/Forecast";

export default function Home() {
  const weatherApi = process.env["NODE_ENV"] == 'production' ? "api" : process.env["services__apiservice__http__0"]

  return (
    <div className="App">
      <header className="App-header">
        <Image
          src={logoRepl}
          className="App-logo"
          alt="logo"
        />
        <Forecast weatherEndpoint={weatherApi + "/weatherforecast"} />
      </header>
    </div>
  );
}