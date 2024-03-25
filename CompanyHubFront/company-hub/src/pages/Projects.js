import "../App.css";
import { useContext, useEffect, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { LoginContext } from "../App";
import DeleteProject from "../components/DeleteProject";
import Project from "../components/Project";
import EditProject from "../components/EditProject";
import AddTask from "../components/AddTask";
import AddProject from "../components/AddProject";
import ProjectTasks from "../components/ProjectTasks";

function Projects() {
  const [loggedIn, setLoggedIn] = useContext(LoginContext);
  const [isAdmin, setIsAdmin] = useState(
    localStorage.getItem("role") === "ADMIN" ? true : false
  );
  const [notFound, setNotFound] = useState(false);
  const [error, setError] = useState(false);
  const [projects, setProjects] = useState();

  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    console.log("Fetching...");
    fetch("https://localhost:7108/Project/GetProjects", {
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
          setProjects(data);
        }
      });
  }, []);

  if (error === true) {
    return (
      <>
        <p>Something went wrong </p>
        <Link to="/Projects">Return</Link>
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
      {isAdmin ? <AddProject /> : null}
      {projects ? (
        <div className="flex flex-wrap justify-center">
          {projects.map((project) => {
            const editProject = (
              <EditProject
                name={project.name}
                description={project.description}
                deadline={project.peadline}
                id={project.id}
              />
            );
            const deleteProject = <DeleteProject id={project.id} />;
            const addTask = <AddTask id={project.id} />;
            const projectTasks = (
              <ProjectTasks id={project.id} name={project.name} />
            );
            return (
              <Project
                key={project.id}
                id={project.id}
                name={project.name}
                description={project.description}
                startDate={project.startDate}
                deadline={project.deadline}
                procentage={project.procentageCompleted}
                deleteProject={deleteProject}
                editProject={editProject}
                addTask={addTask}
                projectTasks={projectTasks}
              />
            );
          })}
        </div>
      ) : null}
    </div>
  );
}

export default Projects;
