import { useContext, useEffect, useState } from "react";
import { LoginContext } from "../App";
import { useNavigate } from "react-router-dom";

export default function ChatMessages(props) {
  const [isSender, setIsSender] = useState();

  function formatirajDatum(inputDate) {
    const parts = inputDate.split("T")[0].split("-");
    return `${parts[2]}.${parts[1]}.${parts[0]}`;
  }

  function check() {
    if (localStorage.getItem("id") === props.sender) {
      return true;
    } else {
      return false;
    }

  }
  return (
    <>
      {props.messages ? (
        <>
          {check() ? (
            <div className="flex justify-end mb-2">
              <div className="rounded py-2 px-3 bg-yellow-100">
                <p className="text-sm mt-1">{props.text}.</p>
                <time
                  className="text-right text-xs text-grey-900 mt-1"
                  dateTime={props.time}
                >
                  {formatirajDatum(props.time)}
                </time>
                {/* <p className="text-right text-xs text-grey-900 mt-1">
                        12:45 pm
                      </p> */}
              </div>
            </div>
          ) : (
            <div className="flex mb-2">
              <div className="rounded py-2 px-3 bg-purple-200">
                {props.group ? (
                  <p className="text-sm text-gray-900"></p>
                ) : null}

                <p className="text-sm mt-1">{props.text}</p>
                {/* <p className="text-right text-xs text-grey-900 mt-1">
                        12:45 pm
                      </p> */}
                <time
                  className="text-right text-xs text-grey-900 mt-1"
                  dateTime={props.time}
                >
                  {formatirajDatum(props.time)}
                </time>
              </div>
            </div>
          )}
        </>
      ) : null}
    </>
  );
}
