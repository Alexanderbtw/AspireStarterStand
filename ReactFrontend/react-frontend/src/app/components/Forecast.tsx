"use client";

import { FC, useEffect, useState } from "react";
import WeatherForecast from "../interfaces/weather-forecast";

type Props = {
  weatherEndpoint: string;
};

const Forecast: FC<Props> = ({ weatherEndpoint: weatherApi }) => {
  const [forecasts, setForecasts] = useState<WeatherForecast[]>();
  
  const fetchData = async (weatherApi: string) => {
    const weather = await fetch(weatherApi);
    if (!weather.ok) return;
    const weatherJson = await weather.json();
    setForecasts(weatherJson);
  };

  useEffect(() => {
    fetchData(weatherApi);
  }, [weatherApi]);

  return (
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
  );
};

export default Forecast;