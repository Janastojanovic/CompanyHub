import React, { useState } from "react";

export default function AddGroupChat({ isOpen, onClose, onAddGroupChat })
 {
    const [selectedUsers, setSelectedUsers] = useState([]);
    const [groupName, setGroupName] = useState("");
  
    const handleUserSelect = (userId) => {
      setSelectedUsers((prevSelectedUsers) => {
        if (prevSelectedUsers.includes(userId)) {
          return prevSelectedUsers.filter((id) => id !== userId);
        } else {
          return [...prevSelectedUsers, userId];
        }
      });
    };
  
    const handleAddGroupChat = () => {
      if (selectedUsers.length >= 2 && groupName.trim() !== "") {
        onAddGroupChat(groupName, selectedUsers);
        onClose();
      } else {
        alert("Please select at least 2 users and provide a group name.");
      }
    };
  
    return (
      <div className={`modal ${isOpen ? "is-active" : ""}`}>
        <div className="modal-background" onClick={onClose}></div>
        <div className="modal-card">
          <header className="modal-card-head">
            <p className="modal-card-title">Add Group Chat</p>
            <button
              className="delete"
              aria-label="close"
              onClick={onClose}
            ></button>
          </header>
          <section className="modal-card-body">
            <div className="field">
              <label className="label">Group Name:</label>
              <div className="control">
                <input
                  className="input"
                  type="text"
                  placeholder="Enter group name"
                  value={groupName}
                  onChange={(e) => setGroupName(e.target.value)}
                />
              </div>
            </div>
            <div className="field">
              <label className="label">Select Users:</label>
              <div className="control">
                {selectedUsers.map((user) => (
                  <label className="checkbox" key={user.id}>
                    <input
                      type="checkbox"
                      checked={selectedUsers.includes(user.id)}
                      onChange={() => handleUserSelect(user.id)}
                    />{" "}
                    {user.name}
                  </label>
                ))}
              </div>
            </div>
          </section>
          <footer className="modal-card-foot">
            <button className="button is-success" onClick={handleAddGroupChat}>
              Add Chat
            </button>
            <button className="button" onClick={onClose}>
              Cancel
            </button>
          </footer>
        </div>
      </div>
    );
  };
  