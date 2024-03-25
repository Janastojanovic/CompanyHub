import { Card, Typography } from "@material-tailwind/react";
import { useEffect, useState } from "react";
import Modal from "react-bootstrap/Modal";
import { Link } from "react-router-dom";
import CompleteTask from "./CompleteTask";
import DeleteTask from "./DeleteTask";
import EditTask from "./EditTask";

export default function ProjectTasks(props) {
  const TABLE_HEAD = [
    "Name",
    "Assigned Date",
    "Deadline",
    "Completed",
    "",
    "",
    "",
  ];
  const [error, setError] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [loading, setLoading] = useState(true);
  const [tasks, setTasks] = useState([]);
  const [projectName, setProjectName] = useState(props.name);

  const handleOpenModal = () => {
    setShowModal(true);
  };

  const handleCloseModal= () => {
    setShowModal(false);
    window.location.reload();
  };

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
    setLoading(true);
    fetch("https://localhost:7108/Task/GetTasksForProject/" + props.id, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
      },
    })
      .then((response) => {
        console.log("Fetching tasks response:", response);
        if (!response.ok) {
          throw new Error("Failed to fetch loans");
        }
        return response.json();
      })
      .then((data) => {
        console.log("Loans data:", data);
        setTasks(data);
        setLoading(false);
      })
      .catch((error) => {
        console.error("Error fetching tasks:", error);
        setError(true);
        setLoading(false);
      });
  }, [props.id]);


  if (error) {
    return (
      <>
        <p>Something went wrong</p>
        <Link to="/projects">Return</Link>
      </>
    );
  }

  return (
    <>
      <button
        onClick={handleOpenModal}
        className="px-4 py-1 text-sm text-purple-600 font-semibold rounded-full border border-purple-200 hover:text-white hover:bg-purple-600 hover:border-transparent focus:outline-none focus:ring-2 focus:ring-purple-600 focus:ring-offset-2"
      >
        Project tasks
      </button>

      <Modal
        size="lg"
        show={showModal}
        onHide={handleCloseModal}
        backdrop="static"
        keyboard={false}
      >
        <Modal.Header closeButton>
          <Modal.Title>{projectName}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
         {tasks.length > 0 ? (
            <Card className="overflow-scroll">
              <table className="table-auto text-left">
                <thead>
                  <tr>
                    {TABLE_HEAD.map((head) => (
                      <th
                        key={head}
                        className="border-b border-blue-gray-100 bg-blue-gray-50 p-4"
                      >
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal leading-none opacity-70"
                        >
                          {head}
                        </Typography>
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {tasks.map((task) => {
                    const rowClassName = task.completed ? "bg-emerald-100" : "";
                    return (
                      <tr
                        key={task.id}
                        className={rowClassName}
                        data-testid={task.id}
                      >
                        <td className="p-4 border-b border-blue-gray-50 bg-blue-gray-50/50">
                          <Typography
                            variant="small"
                            color="blue-gray"
                            className="font-normal"
                          >
                            {task.name}
                          </Typography>
                        </td>
                        <td className="p-4 border-b border-blue-gray-50">
                          <Typography
                            variant="small"
                            color="blue-gray"
                            className="font-normal"
                          >
                            {/* <time dateTime={task.assignedDate}>
                              {task.assignedDate}
                            </time> */}
                            {formatDate(task.assignedDate)}
                          </Typography>
                        </td>
                        <td className="p-4 border-b border-blue-gray-50">
                          <Typography
                            variant="small"
                            color="blue-gray"
                            className="font-normal"
                          >
                            {/* <time dateTime={task.assignedDate}>
                              {task.assignedDate}
                            </time> */}
                            {formatDate(task.deadline)}
                          </Typography>
                        </td>
                        <td className="p-4 border-b border-blue-gray-50">
                          <Typography
                            variant="small"
                            color="blue-gray"
                            className="font-normal"
                          >
                            {task.completed ? <p>Yes</p> : <p>No</p>}
                          </Typography>
                        </td>
                        <td className="p-4 border-b border-blue-gray-50 bg-blue-gray-50/50">
                          {!task.completed ? (
                            <div className="flex">
                              <div className="flex-1">
                                <CompleteTask id={task.id} />
                              </div>
                            </div>
                          ) : null}
                        </td>
                        <td className="p-4 border-b border-blue-gray-50 bg-blue-gray-50/50">
                          <div className="flex-1">
                            <DeleteTask id={task.id} />
                          </div>
                        </td>
                        <td className="p-4 border-b border-blue-gray-50 bg-blue-gray-50/50">
                          <div className="flex-1">
                            <EditTask id={task.id} />
                          </div>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </Card>
          ) : (
            <p>No tasks available</p>
          )}
        </Modal.Body>
        <Modal.Footer>
          <button
            className="bg-slate-400 hover:bg-slate-500 text-white font-bold py-2 px-4 rounded"
            onClick={handleCloseModal}
          >
            Close
          </button>
        </Modal.Footer>
      </Modal>
    </>
  );
}
