import "../App.css";
import { useContext, useEffect, useState } from "react";
import User from "../components/User";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { LoginContext } from "../App";
import DeleteProfile from "../components/DeleteProfile";

function Users() {
  const [loggedIn, setLoggedIn] = useContext(LoginContext);

  const [notFound, setNotFound] = useState(false);
  const [error, setError] = useState(false);
  const [validUsers, setValidUsers] = useState();

  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    console.log("Fetching...");
    fetch("https://localhost:7108/User/GetValidUsers", {
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
          console.log(data);
          console.log("error false");
          setValidUsers(data);
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
      {validUsers ? (
        <div className="flex flex-wrap justify-center">
          {validUsers.map((user) => {
            const deleteInvalidUser = <DeleteProfile id={user.id} />;
            return (
              <User
                key={user.id}
                id={user.id}
                firstname={user.firstname}
                lastname={user.lastname}
                phone={user.phone}
                email={user.email}
                username={user.userName}
                img="https://cdn150.picsart.com/upscale-245339439045212.png"
                deleteInvalidUser={deleteInvalidUser}
                invalid={false}
              />
            );
          })}
        </div>
      ) : null}
    </div>
  );
}

export default Users;
