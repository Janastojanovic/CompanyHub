import "../App.css";
import { useContext, useEffect, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { LoginContext } from "../App";
import EditTask from "../components/EditTask";
import DeleteTask from "../components/DeleteTask";
import CompleteTask from "../components/CompleteTask";
import Task from "../components/Task";

function Tasks() {
  const [loggedIn, setLoggedIn] = useContext(LoginContext);

  const [notFound, setNotFound] = useState(false);
  const [error, setError] = useState(false);
  const [tasks, setTasks] = useState();

  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    console.log("Fetching...");
    fetch("https://localhost:7108/Task/GetTasksForUser/"+localStorage.getItem("id"), {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
      },
    })
      .then((response) => {
        console.log(response.status);
        if (response.status === 404) {
          setNotFound(true);
        } else if (response.status === 401) {
          navigate("/login");
        } else if (response.status === 500) {
          //setServerError(true);
        }
        if (!response.ok) {
          setError(true);
        } else {
          return response.json();
        }
      })
      .then((data) => {
        if (error === false) {
          console.log("error false");
          setTasks(data);
        }
      });
  }, []);

  if (error === true) {
    return (
      <>
        <p>Something went wrong </p>
        <Link to="/Tasks">Return</Link>
      </>
    );
  }
  if (notFound === true) {
    return (
      <>
        <p className="text-purple-800 ">Currently, there are no projects.</p>
      </>
    );
  }
  return (
    <div className="App bg-orange-50 min-h-screen">
      {tasks ? (
        <div className="flex flex-wrap justify-center">
          {tasks.map((task) => {
            const completeTask = (
              <CompleteTask id={task.id} />
            );
            return (
              <Task
                key={task.id}
                id={task.id}
                name={task.name}
                description={task.description}
                startDate={task.assignedDate}
                deadline={task.deadline}
                projectId={task.projectId}
                completeTask={completeTask}
              />
            );
          })}
        </div>
      ) : null}
    </div>
  );
}

export default Tasks;
