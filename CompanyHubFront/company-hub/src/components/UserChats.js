import { useContext, useEffect, useState } from "react";
import { LoginContext } from "../App";
import { useNavigate } from "react-router-dom";
import ChatMessages from "./ChatMessages";
import AddGroupChat from "./AddGroupChat";

export default function UserChats() {
  const [loggedIn, setLoggedIn] = useContext(LoginContext);
  const [userChats, setUserChats] = useState([]);
  const [userChatMaps, setUserChatMaps] = useState([]);
  const [lastMessages, setLastMessages] = useState([]);
  const [recentMessages, setRecentMessages] = useState();
  const [chat, setChat] = useState();
  const [group, setGroup] = useState();
  const [onlineUsers, setOnlineUsers] = useState([]);
  //   const [recentMessages, setRecentMessages] = useState({});
  const [activeChat, setActiveChat] = useState(null);
  const [modalOpen, setModalOpen] = useState(false);

  const [searchValue, setSearchValue] = useState("");
  const [searchResults, setSearchResults] = useState([]);
  const [chatClicked, setChatClicked] = useState();
  const [clicked, setClicked] = useState(false);
  const [plusClicked, setPlusClicked] = useState(false);
  const [noRecentMessages, setNoRecentMessages] = useState(false);
  const [noMessages, setNoMessages] = useState(false);
  const [inputText, setInputText] = useState("");

  const navigate = useNavigate();

  function isOnline(userId) {
    // console.log("isONLINE:");
    // console.log(onlineUsers);
    return onlineUsers.some((user) => user.id === userId);
  }
  const openModal = () => {
    setModalOpen(true);
  };

  function handleIconClick(e) {
    const message = inputText.trim();
    console.log(chatClicked);
    console.log("usloUSLOOOOOOOOOOoo");
    console.log(localStorage.getItem("accessToken"));
    e.preventDefault();
    const url = "https://localhost:7108/Message/AddMessage/" + chatClicked;
    fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
      },
      body: JSON.stringify({
        text: message,
        chatId: chatClicked,
        senderId: localStorage.getItem("id"),
      }),
    })
      .then((response) => {
        if (!response.ok) {
          if (response.status === 400) {
            alert("Can't send empty message");
          } else {
            alert("Something is wrong");
          }
        } else {
          handleChatClick(chatClicked);
          setInputText(""); // Postaviti inputText na prazan string
          document.getElementById("input-msg").value = ""; // Opciono, ali možete dodati i ovo da biste očistili input polje
        }
      })
      .then((data) => {
        console.log(data);
      });
  }

  useEffect(() => {
    // Funkcija za dobijanje svih razgovora korisnika
    const fetchUserChats = async () => {
      try {
        const response = await fetch(
          `https://localhost:7108/Chat/GetChatsForUser/${localStorage.getItem(
            "id"
          )}`,
          {
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
            },
          }
        );
        if (!response.ok) {
          if (response.status === 401) {
            navigate("/login");
          } else if (response.status === 404) {
            setUserChats([]);
          } else {
            throw new Error("Failed to fetch user chats");
          }
        } else {
          const data = await response.json();
          setUserChats(data);
        }
      } catch (error) {
        console.error("Error fetching user chats:", error);
      }
    };

    fetchUserChats();

    if (chatClicked) {
      fetchUserChats();
    }
  }, [navigate, chatClicked]);

  useEffect(() => {
    // Funkcija za dobijanje svih razgovora korisnika
    const fetchOnlineUsers = async () => {
      try {
        const response = await fetch(
          `https://localhost:7108/User/GetOnlineUsers`,
          {
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
            },
          }
        );
        if (!response.ok) {
          if (response.status === 401) {
            navigate("/login");
          } else if (response.status === 404) {
            setOnlineUsers([]);
          } else {
            throw new Error("Failed to fetch online users");
          }
        } else {
          const data = await response.json();
          setOnlineUsers(data);
          //   console.log("OVO SU ONLINE:");
          //   console.log(onlineUsers);
        }
      } catch (error) {
        console.error("Error fetching online users:", error);
      }
    };

    fetchOnlineUsers();
  }, [navigate]);

  useEffect(() => {
    // Funkcija za dobijanje korisnika u svakom razgovoru
    const fetchUserChatMaps = async (chatId) => {
      try {
        const response = await fetch(
          `https://localhost:7108/Chat/GetChatMapsForChat/${chatId}`,
          {
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
            },
          }
        );
        if (!response.ok) {
          throw new Error("Failed to fetch user chat maps");
        } else {
          const data = await response.json();
          setUserChatMaps((prevUserChatMaps) => ({
            ...prevUserChatMaps,
            [chatId]: data,
          }));
          //   console.log(userChatMaps);
        }
      } catch (error) {
        console.error("Error fetching user chat maps:", error);
      }
    };

    // Funkcija za dobijanje poslednje poruke u svakom razgovoru
    const fetchLastMessage = async (chatId) => {
      try {
        const response = await fetch(
          `https://localhost:7108/Message/GetLastMessage/` + chatId,
          {
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
            },
          }
        );
        if (!response.ok) {
          //  setNoMessages(true);
        } else {
          const data = await response.json();
          setLastMessages((prevLastMessages) => ({
            ...prevLastMessages,
            [chatId]: data.message,
          }));
        }
      } catch (error) {
        console.error("Error fetching last message:", error);
      }
    };

    userChats.forEach((chat) => {
      fetchUserChatMaps(chat.id);
      fetchLastMessage(chat.id);
    });
    // fetchOnlineUsers();
  }, [userChats]);

  function getLast(chatId) {
    const url = `https://localhost:7108/Message/GetLastMessage/` + chatId;
    return fetch(url, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
      },
    }).then((response) => {
      if (response.status !== 200) {
        //setError(true);
      } else {
        return response.json().then((data) => data.message);
      }
    });
  }

  useEffect(() => {
    const fetchMessages = async () => {
      try {
        // Fetch messages for the currently selected chat
        const response = await fetch(
          `https://localhost:7108/Message/GetRecentMessages/${chatClicked}`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
            },
          }
        );
        if (response.ok) {
          const data = await response.json();
          setRecentMessages(data);
        }
      } catch (error) {
        console.error("Error fetching recent messages:", error);
      }
    };

    // Fetch messages initially
    if (chatClicked) {
      fetchMessages();

      // Poll for new messages every 5 seconds (adjust as needed)
      const interval = setInterval(fetchMessages, 5000);

      // Cleanup interval on component unmount
      return () => clearInterval(interval);
    }
  }, [chatClicked]);

  const handleChatClick = async (chatId) => {
    try {
      setChatClicked(chatId);
      const response = await fetch(
        `https://localhost:7108/Message/GetRecentMessages/` + chatId,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
          },
        }
      );
      if (!response.ok) {
        if (response.status === 404) {
          setNoRecentMessages(true);
        } else {
          throw new Error("Failed to handle chat click");
        }
      } else {
        const data = await response.json();
        // setRecentMessages((prevRecentMessages) => ({
        //   ...prevRecentMessages,
        //   [chatId]: data, // Dodajte poruke pod ključem chatId
        // }));
        setRecentMessages(data);
      }
      setActiveChat(chatId);
    } catch (error) {
      console.error("Error handling chat click:", error);
    }
  };

  const handleChatClick1 = async (userId) => {
    try {
      const response = await fetch(
        "https://localhost:7108/Chat/AddChat/" +
          localStorage.getItem("id") +
          "/" +
          userId,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
          },
        }
      );
      if (!response.ok) {
        if (response.status === 400) {
          alert("Can't make chat with yourself");
        } else {
          throw new Error("Failed to handle chat click");
        }
      } else {
        const data = await response.json();
        // setRecentMessages((prevRecentMessages) => ({
        //   ...prevRecentMessages,
        //   [chatId]: data, // Dodajte poruke pod ključem chatId
        // }));
        setChatClicked(data.id);
        //setRecentMessages(data);
        setActiveChat(data.id);

        handleChatClick(data.id);
      }
    } catch (error) {
      console.error("Error handling chat click:", error);
    }
  };
  function handlePlusClick() {
    // openModal();
    console.log("Plus icon clicked");
    setModalOpen(true);
  }

  const closeModal = () => {
    setModalOpen(false);
  };
  const handleSearchChange = (event) => {
    console.log("USLOOOOOO");
    setClicked(true);
    setSearchValue(event.target.value);
    if (event.target.value.trim() !== "") {
      fetch(
        `https://localhost:7108/User/SearchByName?search=${event.target.value}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
          },
        }
      )
        .then((response) => response.json())
        .then((data) => setSearchResults(data))
        .catch((error) => console.error("Error fetching data:", error));
    } else {
      setSearchResults([]);
      setClicked(false);
    }
    //console.log(searchResults);
  };
  const handleAddGroupChat = (groupName, selectedUsers) => {
    fetch("https://localhost:7108/Chat/AddGroupChat?chatName" + groupName, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
      },
      body: JSON.stringify({
        users: selectedUsers,
      }),
    })
      .then((response) => {
        if (response.status !== 200) {
          if (response.status === 400) {
            return response.json();
          }
        } else {
          return response.json();
        }
      })
      .then((data) => {
        if (data && data.message !== undefined) {
          console.log(data.message);
          alert(data.message);
          window.location.reload();
        }
      });
    console.log("Adding group chat:", groupName, selectedUsers);
  };

  return (
    <>
      <div className="w-1/3 border flex flex-col overflow-auto">
        {/* Ostatak komponente */}
        <div className="py-2 px-3 bg-purple-100 flex flex-row justify-between items-center">
          <div className="flex">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="purple"
              className="w-5 h-5"
              //   onClick={handlePlusClick}
            >
              <path d="M10.75 4.75a.75.75 0 0 0-1.5 0v4.5h-4.5a.75.75 0 0 0 0 1.5h4.5v4.5a.75.75 0 0 0 1.5 0v-4.5h4.5a.75.75 0 0 0 0-1.5h-4.5v-4.5Z" />
            </svg>

            {/* {modalOpen && (
              <AddGroupChat
                isOpen={modalOpen}
                onClose={closeModal}
                onAddGroupChat={handleAddGroupChat}
              />
              // Implementacija modal komponente
            )} */}
          </div>
        </div>
        <div className="py-2 px-2 bg-orange-50">
          <input
            type="text"
            className="bg-emerald-50 appearance-none border-2 border-gray-200 rounded w-full py-2 px-4 text-gray-700 leading-tight focus:outline-none focus:bg-white focus:border-purple-500"
            placeholder="Search or start new chat"
            onChange={handleSearchChange}
          />
          <div id="searchResults">
            {clicked ? (
              <>
                {searchResults.length > 0 ? (
                  searchResults.map((user, index) => (
                    <div
                      key={index}
                      className="bg-purple-50 flex-1 overflow-auto"
                      onClick={() => handleChatClick1(user.id)}
                    >
                      <div className="px-3 flex items-center hover:bg-purple-100">
                        <div className="ml-4 flex-1 border-b border-gray-400 py-4">
                          {user.firstname} {user.lastname}
                        </div>
                      </div>
                    </div>
                    // Ovde prilagodite prikaz rezultata pretrage kako vam odgovara
                  ))
                ) : (
                  <div>Nema rezultata.</div>
                )}
              </>
            ) : (
              <>
                <div>
                  {userChats.length > 0 ? (
                    userChats.map((chat) => (
                      <div
                        key={chat.id}
                        className="bg-purple-50 flex-1 overflow-hidden"
                        onClick={() => handleChatClick(chat.id)}
                      >
                        <div
                          className={`px-3 flex items-center hover:bg-purple-100 ${
                            activeChat === chat.id ? "bg-purple-100" : "" // Dodajte klasu koja definiše boju na kliknutom četu
                          }`}
                        >
                          <div className="ml-4 flex-1 border-b border-gray-400 py-4">
                            <div>
                              {/* ovde iznad div je levodesno problem className="flex items-bottom justify-between" */}
                              {userChatMaps[chat.id] &&
                                userChatMaps[chat.id].length > 0 &&
                                (userChatMaps[chat.id].length > 2 ? (
                                  <>
                                    <p className="text-gray-900">{chat.name}</p>
                                    {/* <p className="text-gray-900 mt-1 text-sm">
                              {lastMessages[chat.id]}
                            </p> */}
                                  </>
                                ) : (
                                  <>
                                    {userChatMaps[chat.id].map(
                                      (userChatMap, index) => (
                                        <div
                                          key={index}
                                          className="flex items-center"
                                        >
                                          {/* <p className="text-gray-900">
                                  {userChatMap.userId !==
                                    localStorage.getItem("id") &&
                                    userChatMap.userName}
                                </p> */}
                                          {userChatMap.userId !==
                                            localStorage.getItem("id") && (
                                            <p className="text-gray-900">
                                              {userChatMap.userName}
                                            </p>
                                          )}

                                          {userChatMap.userId !==
                                            localStorage.getItem("id") &&
                                            isOnline(userChatMap.userId) && (
                                              <div className="w-3.5 flex-none rounded-full bg-emerald-500/20 p-1 ml-2 -mt-2.5">
                                                <div className="h-1.5 w-1.5 rounded-full bg-emerald-500"></div>
                                              </div>
                                            )}
                                        </div>
                                      )
                                    )}
                                  </>
                                ))}
                            </div>

                            {lastMessages[chat.id]}
                          </div>
                        </div>
                      </div>
                    ))
                  ) : (
                    <p className="text-purple-800">
                      Currently, user doesn't have chats
                    </p>
                  )}
                </div>
              </>
            )}
          </div>
        </div>
      </div>
      {noRecentMessages ? (
        <ChatMessages messages={false} />
      ) : (
        <>
          {recentMessages ? (
            <>
              <div className="w-2/3 border flex flex-col bg-orange-100">
                <div className="py-2 px-3 bg-grey-100 flex flex-row justify-between items-center">
                  <div className="flex items-center ">
                    <div className="ml-4 ">
                      <p className="text-grey-900">Chat</p>
                      <p className="text-grey-900 text-xs mt-1"></p>
                    </div>
                  </div>

                  <div className="flex">
                    <div className="ml-6">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        viewBox="0 0 24 24"
                        width="24"
                        height="24"
                      >
                        <path
                          fill="#263238"
                          fill-opacity=".6"
                          d="M12 7a2 2 0 1 0-.001-4.001A2 2 0 0 0 12 7zm0 2a2 2 0 1 0-.001 3.999A2 2 0 0 0 12 9zm0 6a2 2 0 1 0-.001 3.999A2 2 0 0 0 12 15z"
                        ></path>
                      </svg>
                    </div>
                  </div>
                </div>

                <div className="flex-1 overflow-auto bg-purple-50 ">
                  <div className="py-2 px-3">
                    <div className="flex justify-center mb-2"></div>
                    <div className="flex justify-center mb-4"></div>
                    {recentMessages.map((recentMessage) => {
                      //console.log(chatClicked)
                      return (
                        <ChatMessages
                          key={recentMessage.id}
                          group={
                            userChatMaps[recentMessage.chatId].length > 2
                              ? true
                              : false
                          }
                          messages={true}
                          id={recentMessage.id}
                          text={recentMessage.text}
                          sender={recentMessage.senderId}
                          time={recentMessage.time}
                        />
                      );
                    })}
                  </div>
                </div>
                <div className="bg-emerald-50 px-4 py-4 flex items-center">
                  <div>
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      viewBox="0 0 24 24"
                      width="24"
                      height="24"
                    >
                      <path
                        opacity=".45"
                        fill="#263238"
                        d="M9.153 11.603c.795 0 1.439-.879 1.439-1.962s-.644-1.962-1.439-1.962-1.439.879-1.439 1.962.644 1.962 1.439 1.962zm-3.204 1.362c-.026-.307-.131 5.218 6.063 5.551 6.066-.25 6.066-5.551 6.066-5.551-6.078 1.416-12.129 0-12.129 0zm11.363 1.108s-.669 1.959-5.051 1.959c-3.505 0-5.388-1.164-5.607-1.959 0 0 5.912 1.055 10.658 0zM11.804 1.011C5.609 1.011.978 6.033.978 12.228s4.826 10.761 11.021 10.761S23.02 18.423 23.02 12.228c.001-6.195-5.021-11.217-11.216-11.217zM12 21.354c-5.273 0-9.381-3.886-9.381-9.159s3.942-9.548 9.215-9.548 9.548 4.275 9.548 9.548c-.001 5.272-4.109 9.159-9.382 9.159zm3.108-9.751c.795 0 1.439-.879 1.439-1.962s-.644-1.962-1.439-1.962-1.439.879-1.439 1.962.644 1.962 1.439 1.962z"
                      ></path>
                    </svg>
                  </div>
                  <div className="flex-1 mx-4">
                    <input
                      id="input-msg"
                      className="w-full appearance-none border-2 border-gray-200 rounded px-2 py-2 text-gray-700 leading-tight focus:outline-none focus:bg-white focus:border-purple-500"
                      type="text"
                      onChange={(e) => setInputText(e.target.value)}
                    />
                  </div>
                  <div>
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      fill="none"
                      viewBox="0 0 24 24"
                      strokeWidth={1.5}
                      stroke="currentColor"
                      className="w-6 h-6 hover:stroke-purple-700"
                      onClick={handleIconClick}
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        d="M6 12 3.269 3.125A59.769 59.769 0 0 1 21.485 12 59.768 59.768 0 0 1 3.27 20.875L5.999 12Zm0 0h7.5"
                      />
                    </svg>
                  </div>
                </div>
              </div>
            </>
          ) : null}
        </>
      )}
    </>
  );
  
}
