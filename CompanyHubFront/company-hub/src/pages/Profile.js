import "../App.css";
import { useContext, useEffect, useState } from "react";
import { v4 as uuidv4 } from "uuid";
import Header from "../components/Header";
import { Link, useLocation, useNavigate } from "react-router-dom";
import NotFound from "../components/NotFound";
import { LoginContext } from "../App";
import Prof from "../components/Prof";
import "bootstrap/dist/css/bootstrap.min.css";

function Profile() {
  const navigate = useNavigate();
  const [notFound, setNotFound] = useState(false);
  const [error, setError] = useState(false);
  const [user, setUser] = useState();
  
  useEffect(() => {
    console.log("Fetching...");
    console.log(localStorage.getItem("accessToken"));
    console.log(localStorage.getItem("role"));
    fetch(
      "https://localhost:7108/User/GetUser?id=" + localStorage.getItem("id"),
      {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
        },
      }
    )
      .then((response) => {
        if (response.status === 404) {
          setNotFound(true);
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
        console.log(data);
        if (error === false) {
          setUser(data);
        }
      });
  }, []);

  if (error === true) {
    return (
      <>
        <p>Something went wrong </p>
        <Link to="/Home">Return</Link>
      </>
    );
  }
  if (notFound === true) {
    return (
      <>
        <NotFound />
        <Link to="/Home">Return</Link>
      </>
    );
  }
  return (
    <div className="App bg-orange-50 min-h-screen">
      {user ? (
        <div className="flex flex-wrap justify-center">
          <div>
            {/* {setRole(user.id)} */}
            <Prof
              key={user.id}
              id={user.id}
              firstname={user.firstname}
              lastname={user.lastname}
              phone={user.phone}
              email={user.email}
              username={user.username}
              img="https://cdn150.picsart.com/upscale-245339439045212.png"
              //user={user}
            />
          </div>
        </div>
      ) : null}
    </div>
  );
}

export default Profile;
