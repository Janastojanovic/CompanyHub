import { useState, useEffect, useContext } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { LoginContext } from "../App";

export default function Login() {
  const [loggedIn, setLoggedIn] = useContext(LoginContext);
  const [email, setEmail] = useState();
  const [password, setPassword] = useState();
  const [error, setError] = useState(false);

  const location = useLocation();
  const navigate = useNavigate();

  const setRole = async (id) => {
    console.log(localStorage.getItem("accessToken"));

    console.log(localStorage);
    console.log("USLO");
    const url = "https://localhost:7108/api/v1/authenticate/CheckRole/" + id;
    fetch(url, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
      },
    })
      .then((response) => {
        if (response.status === 401) {
          navigate("/login");
        }
        if (!response.ok) {
          alert("Something is wrong");
        } else {
          return response.json();
        }
      })
      .then((data) => {
        if (data && data.role !== undefined) {
          localStorage.setItem("role", data.role);
        }
        console.log("ROLE:");
        console.log(localStorage.getItem("role"));
      });
  }

  const login = async (e) =>{
    e.preventDefault();
    const url = "https://localhost:7108/api/v1/authenticate/login";
    fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        email: email,
        password: password,
      }),
    })
      .then((response) => {
        if (response.status !== 200) {
          if (response.status === 400) {
            return response.json();
          }
          setError(true);
        } else {
          return response.json();
        }
      })
      .then((data) => {
        if (error !== true) {
          if (data && data.accessToken) {
            localStorage.setItem("accessToken", data.accessToken);
            localStorage.setItem("id", data.userId);
            setRole(data.userId);
            setLoggedIn(true);
            navigate(
              location?.state?.previousUrl ? location.state.previousUrl : "/"
            );
          } else if (data && data.message !== undefined) {
            console.log(data.message);
            alert(data.message);
            window.location.reload();
          } else {
            console.error("Access token not found in response data.");
          }
          // console.log("LOCALSTORAGE");
          // console.log(localStorage);
          // console.log(localStorage.getItem("id"));
          
        }
      });
  }
  if (error === true) {
    return (
      <>
        <Login />
      </>
    );
  }
  return (
    <form className="m-2 w-full max-w-sm" id="customer" onSubmit={login}>
      <div className="md:flex md:items-center mb-6">
        <div className="md:w-1/4">
          <label for="email">Email</label>
        </div>

        <div className="md:w-3/4">
          <input
            className="bg-gray-200 appearance-none border-2 border-gray-200 rounded w-full py-2 px-4 text-gray-700 leading-tight focus:outline-none focus:bg-white focus:border-purple-500"
            id="email"
            type="text"
            value={email}
            onChange={(e) => {
              setEmail(e.target.value);
            }}
          />
        </div>
      </div>

      <div className="md:flex md:items-center mb-6">
        <div className="md:w-1/4">
          <label for="password">Password</label>
        </div>

        <div className="md:w-3/4">
          <input
            id="password"
            className="bg-gray-200 appearance-none border-2 border-gray-200 rounded w-full py-2 px-4 text-gray-700 leading-tight focus:outline-none focus:bg-white focus:border-purple-500"
            type="password"
            value={password}
            onChange={(e) => {
              setPassword(e.target.value);
            }}
          />
        </div>
      </div>
      <button className="bg-purple-600 hover:bg-purple-700 text-white font-bold py-2 px-4 rounded">
        Login
      </button>
    </form>
  );
}
