import "../App.css";
import { useContext, useEffect, useState } from "react";
import User from "../components/User";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { LoginContext } from "../App";
import DeleteInvalidUser from "../components/DeleteInvalidUser";
import ApproveUser from "../components/ApproveUser";

function InvalidUsers() {
  const [loggedIn, setLoggedIn] = useContext(LoginContext);

  const [notFound, setNotFound] = useState(false);
  const [error, setError] = useState(false);
  const [invalidUsers, setInvalidUsers] = useState();

  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    console.log("Fetching...");
    fetch("https://localhost:7108/User/GetInvalidUsers", {
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
          setInvalidUsers(data);
        }
      });
  }, []);

  if (error === true) {
    return (
      <>
        <p>Something went wrong </p>
        <Link to="/InvalidUsers">Return</Link>
      </>
    );
  }
  if (notFound === true) {
    return (
      <>
        <p>Currently, there are no invalid users.</p>
      </>
    );
  }
  return (
    <div className="App bg-orange-50 min-h-screen">
      {invalidUsers ? (
        <div className="flex flex-wrap justify-center">
          {invalidUsers.map((invalidUser) => {
            const deleteInvalidUser = <DeleteInvalidUser id={invalidUser.id} />;
            return (
              <User
                key={invalidUser.id}
                id={invalidUser.id}
                firstname={invalidUser.firstname}
                lastname={invalidUser.lastname}
                phone={invalidUser.phone}
                email={invalidUser.email}
                username={invalidUser.userName}
                img="https://cdn150.picsart.com/upscale-245339439045212.png"
                deleteInvalidUser={deleteInvalidUser}
                invalid={true}
              />
            );
          })}
        </div>
      ) : null}
    </div>
  );
}

export default InvalidUsers;
