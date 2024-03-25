import React from "react";
import UserChats from "../components/UserChats";

function Chat() {
  return (
    <div>
      {/* <div className="w-full h-32 bg-purple-200"></div> */}

      <div className="container mx-auto mt-128px;">
        <div className="py-1 h-screen">
          <div className="flex border border-grey rounded shadow-lg h-full">
            <UserChats/>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Chat;
