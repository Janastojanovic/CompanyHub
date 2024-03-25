import { stringify } from "uuid";
import { useContext, useEffect, useState } from "react";
import styles from "./Project.css";
import { LoginContext } from "../App";

function Task(props) {
  const [projectName, setProjectName] = useState();

  function formatDate(dateString) {
    const date = new Date(dateString);
    const day = date.getDate();
    const month = date.getMonth() + 1;
    const year = date.getFullYear();

    // Dodajemo nule ispred dana i mjeseca ako je potrebno
    const formattedDay = day < 10 ? `0${day}` : day;
    const formattedMonth = month < 10 ? `0${month}` : month;

    return `${formattedDay}.${formattedMonth}.${year}`;
  }
  useEffect(() => {
    const url =
      "https://localhost:7108/Project/GetProjectName/" + props.projectId;
    fetch(url, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
      },
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error("Failed to fetch member name");
        }
        return response.json();
      })
      .then((data) => {
        setProjectName(data.name);
      })
      .catch((error) => {
        console.error("Error fetching member name:", error);
      });
  },[]);
  return (
    <div className="m-2 py-8 px-8 max-w-sm bg-white rounded-xl shadow-lg space-y-2 sm:py-4 sm:flex sm:items-center sm:space-y-0 sm:space-x-6">
      <div className="text-center space-y-2 sm:text-left">
        <div className="space-y-0.5">
          <p className="text-lg text-black font-semibold">
            {projectName}
          </p>
          <p className="text-slate-500 font-medium">Name: {props.name}</p>
          <p className="text-slate-500 font-medium">
            Description: {props.description}
          </p>
          <p className="text-slate-500 font-medium">
            Start date: {formatDate(props.startDate)}
          </p>
          <p className="text-slate-500 font-medium">
            Deadline: {formatDate(props.deadline)}
          </p>
        </div>
        {props.completeTask}
      </div>
    </div>
  );
}
export default Task;
