'use client'

import { FC, useEffect, useState } from "react";
import WeatherForecast from "./interfaces/weather-forecast";
import Image from "next/image";
import logo from "./logo.svg";

interface Props {
  weatherApi: string;
}

const Home: FC<Props> = ({
  weatherApi,
}) => {
  const [forecasts, setForecasts] = useState<WeatherForecast[]>([]);

  const fetchData = async (weatherApi: string) => {
    const weather = await fetch(weatherApi);
    const weatherJson = await weather.json();
    setForecasts(weatherJson);
  };

  useEffect(() => {
    fetchData(weatherApi);
  }, [weatherApi]);
  return (
    <div className="App">
      <header className="App-header">
        <Image
          src={logo}
          className="App-logo"
          alt="logo"
          title={forecasts?.length.toString()}
        />
        <table>
          <thead>
            <tr>
              <th>Date</th>
              <th>Temp. (C)</th>
              <th>Temp. (F)</th>
              <th>Summary</th>
            </tr>
          </thead>
          <tbody>
            {(
              forecasts ?? [
                {
                  date: "N/A",
                  temperatureC: "",
                  temperatureF: "",
                  summary: "No forecasts",
                },
              ]
            ).map((w) => {
              return (
                <tr key={w.date.toString()}>
                  <td>{w.date.toString()}</td>
                  <td>{w.temperatureC}</td>
                  <td>{w.temperatureF}</td>
                  <td>{w.summary}</td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </header>
    </div>
  );
}

export default Home;