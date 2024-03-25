import { useContext, useState } from "react";
import DeleteProfile from "./DeleteProfile";
import EditProfile from "./EditProfile";
import "bootstrap/dist/css/bootstrap.min.css";
import { LoginContext } from "../App";
import ChangePassword from "./ChangePassword";

function Prof(props) {
  // function checkRole() {
  //   console.log("usloooooo")
  //   console.log(localStorage.getItem("role"))
  //   if (localStorage.getItem("role") === "ADMIN") {
  //     return true;
  //   } else {
  //     return false;
  //   }
  // }
  const [isAdmin, setIsAdmin] = useState(
    (localStorage.getItem("role")==="ADMIN")? true:false
  )

  return (
    <div className="m-2 py-8 px-8 max-w-sm bg-white rounded-xl shadow-lg space-y-2 sm:py-4 sm:flex sm:items-center sm:space-y-0 sm:space-x-6">
      <img
        className="object-cover rounded-full h-[100px] w-[100px] block mx-auto h-24 rounded-full sm:mx-0 sm:shrink-0"
        src={props.img}
        alt="https://cdn150.picsart.com/upscale-245339439045212.png"
      />
      <div className="text-center space-y-2 sm:text-left">
        <div className="space-y-0.5">
          <p className="text-lg text-black font-semibold">
            {props.firstname} {props.lastname}
          </p>
          <p className="text-slate-500 font-medium">Email: {props.email}</p>
          <p className="text-slate-500 font-medium">
            Username: {props.username}
          </p>
          {/* <p className="text-slate-500 font-medium">Savings: {props.savings}</p> */}
          <p className="text-slate-500 font-medium">Phone: {props.phone}</p>
        </div>
        <EditProfile
          firstname={props.firstname}
          lastname={props.lastname}
          phone={props.phone}
          id={props.id}
        />
        <ChangePassword/>
        {isAdmin ? null : <DeleteProfile id={props.id} />}
      </div>
    </div>
  );
}
export default Prof;
