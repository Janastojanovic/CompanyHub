import "./App.css";
import { createContext, useState } from "react";
import Header from "./components/Header";
// import Categories from "./pages/Categories";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Home from "./pages/Home";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Profile from "./pages/Profile";
import NotFound from "./components/NotFound";
import InvalidUsers from "./pages/InvalidUsers";
import Users from "./pages/Users";
import Chat from "./pages/Chat";
import Projects from "./pages/Projects";
import Tasks from "./pages/Tasks";
// import Transactions from "./pages/Transactions";
// import NotFound from "./components/NotFound";
// import Login from "./pages/Login";
// import Register from "./pages/Register";
// import Home from "./pages/Home";
// import Profile from "./pages/Profile";

export const LoginContext = createContext();

function App() {
  const [loggedIn, setLoggedIn] = useState(
    localStorage.accessToken ? true : false
  );
  // const [isAdmin,setIsAdmin]= useState(
  //   (localStorage.getItem("role")==="ADMIN")? true:false
  // );

  function changeLoggedIn(value) {
    setLoggedIn(value);
    if (value === false) {
      console.log("Brisem token...");
      localStorage.clear();
      console.log(localStorage)
    }
  }

  return (
    <LoginContext.Provider value={[loggedIn, changeLoggedIn]}>
      <BrowserRouter>
        <Header>
          <Routes>
            {/* <Route path="/categories" element={<Categories />} />
            <Route path="/transactions" element={<Transactions />} /> */}
            <Route path="/" element={<Home />} />
            <Route path="/Home" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/profile" element={<Profile />} />
            <Route path="/invalidUsers" element={<InvalidUsers />} />
            <Route path="/users" element={<Users />} />
            <Route path="/chat" element={<Chat />} />
            <Route path="/projects" element={<Projects />} />
            <Route path="/tasks" element={<Tasks />} />
            <Route path="/404" element={<NotFound />} />
            <Route path="*" element={<NotFound />} />
          </Routes>
        </Header>
      </BrowserRouter>
   </LoginContext.Provider>
  );
}

export default App;
