/* This example requires Tailwind CSS v2.0+ */
import { useContext, useEffect, useState } from "react";
import { Disclosure, Menu, Transition } from "@headlessui/react";
import { BellIcon, MenuIcon, XIcon } from "@heroicons/react/outline";
import { NavLink } from "react-router-dom";
import { LoginContext } from "../App";
import { Fragment } from "react";

const navigation = [
  { name: "Home", href: "/Home" },
  //{ name: "Chat", href: "/chat" },
  //   { name: "Transactions", href: "/Transactions" },
];

function classNames(...classes) {
  return classes.filter(Boolean).join(" ");
}

const setOffline = async (id) => {
  const url =
    "https://localhost:7108/User/SetOnlineStatus?id=" + id + "&status=false";
  console.log(url);
  fetch(url, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
    },
  })
    .then((response) => {
      if (!response.ok) {
        alert("Something is wrong");
        //showCategories = false;
        //throw new Error("Something went wrong");
      }
    })
    .then((data) => {
      console.log(data);
    });
};
export default function Header(props) {
  const [loggedIn, setLoggedIn] = useContext(LoginContext);

  const [isAdmin, setIsAdmin] = useState(
    localStorage.getItem("role") === "ADMIN" ? true : false
  );

  return (
    <>
      <Disclosure as="nav" className="bg-purple-800">
        {({ open }) => (
          <>
            <div className="max-w-7xl mx-auto px-2 sm:px-6 lg:px-8">
              <div className="relative flex items-center justify-between h-14">
                <div className="absolute inset-y-0 left-0 flex items-center sm:hidden">
                  {/* Mobile menu button*/}
                  <Disclosure.Button className="inline-flex items-center justify-center p-2 rounded-md text-gray-400 hover:text-white hover:bg-purple-900 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-white">
                    <span className="sr-only">Open main menu</span>
                    {open ? (
                      <XIcon className="block h-6 w-6" aria-hidden="true" />
                    ) : (
                      <MenuIcon className="block h-6 w-6" aria-hidden="true" />
                    )}
                  </Disclosure.Button>
                </div>
                <div className="flex-1 flex items-center justify-center sm:items-stretch sm:justify-start">
                  <div className="hidden sm:block sm:ml-6">
                    <div className="flex space-x-4">
                      {navigation.map((item) => (
                        <NavLink
                          key={item.name}
                          to={item.href}
                          className={({ isActive }) => {
                            return (
                              "px-3 py-2 rounded-md text-sm font-medium no-underline " +
                              (!isActive
                                ? " text-gray-300 hover:bg-purple-900 hover:text-white"
                                : "bg-purple-900 text-white")
                            );
                          }}
                        >
                          {item.name}
                        </NavLink>
                      ))}
                      {loggedIn ? (
                        <>
                          <NavLink
                            to={"/chat"}
                            className="px-3 py-2 rounded-md text-sm font-medium no-underline text-gray-300 hover:bg-purple-900 hover:text-white"
                          >
                            Chat
                          </NavLink>
                          <NavLink
                            to={"/projects"}
                            className="px-3 py-2 rounded-md text-sm font-medium no-underline text-gray-300 hover:bg-purple-900 hover:text-white"
                          >
                            Projects
                          </NavLink>
                          {isAdmin ? (
                            <>
                              <NavLink
                                to={"/invalidUsers"}
                                className="px-3 py-2 rounded-md text-sm font-medium no-underline text-gray-300 hover:bg-purple-900 hover:text-white"
                              >
                                Requests
                              </NavLink>
                              <NavLink
                                to={"/users"}
                                className="px-3 py-2 rounded-md text-sm font-medium no-underline text-gray-300 hover:bg-purple-900 hover:text-white"
                              >
                                Users
                              </NavLink>
                            </>
                          ) : (
                            <NavLink
                              to={"/tasks"}
                              className="px-3 py-2 rounded-md text-sm font-medium no-underline text-gray-300 hover:bg-purple-900 hover:text-white"
                            >
                              Tasks
                            </NavLink>
                          )}
                        </>
                      ) : (
                        <>
                          <NavLink
                            to={"/login"}
                            className="px-3 py-2 rounded-md text-sm font-medium no-underline text-gray-300 hover:bg-purple-900 hover:text-white"
                          >
                            Login
                          </NavLink>
                          <NavLink
                            to={"/register"}
                            className="px-3 py-2 rounded-md text-sm font-medium no-underline text-gray-300 hover:bg-purple-900 hover:text-white"
                          >
                            Register
                          </NavLink>
                        </>
                      )}
                    </div>
                  </div>
                </div>
                <div className="absolute inset-y-0 right-0 flex items-center pr-2 sm:static sm:inset-auto sm:ml-6 sm:pr-0">
                  {loggedIn ? (
                    <Menu as="div" className="relative ml-3">
                      <div>
                        <Menu.Button className="relative flex rounded-full bg-purple-800 text-sm focus:outline-none focus:ring-2 focus:ring-white focus:ring-offset-2 focus:ring-offset-gray-800">
                          <span className="absolute -inset-1.5" />
                          <span className="sr-only">Open user menu</span>
                          <img
                            src="https://cdn150.picsart.com/upscale-245339439045212.png"
                            alt="Profile"
                            className="block h-6 w-6 rounded-full"
                          />
                        </Menu.Button>
                      </div>
                      <Transition
                        as={Fragment}
                        enter="transition ease-out duration-100"
                        enterFrom="transform opacity-0 scale-95"
                        enterTo="transform opacity-100 scale-100"
                        leave="transition ease-in duration-75"
                        leaveFrom="transform opacity-100 scale-100"
                        leaveTo="transform opacity-0 scale-95"
                      >
                        <Menu.Items className="absolute right-0 z-10 mt-2 w-48 origin-top-right rounded-md bg-white py-1 shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none">
                          <Menu.Item>
                            {({ active }) => (
                              <NavLink
                                to="/profile"
                                className={classNames(
                                  active ? "bg-gray-100" : "",
                                  "block px-4 py-2 text-sm text-gray-700 no-underline"
                                )}
                              >
                                Your profile
                              </NavLink>
                            )}
                          </Menu.Item>
                          <Menu.Item>
                            {({ active }) => (
                              <NavLink
                                to={"/login"}
                                onClick={() => {
                                  setOffline(localStorage.getItem("id"));
                                  setLoggedIn(false);
                                  localStorage.clear();
                                }}
                                className={classNames(
                                  active ? "bg-gray-100" : "",
                                  "block px-4 py-2 text-sm text-gray-700 no-underline"
                                )}
                              >
                                Logout
                              </NavLink>
                            )}
                          </Menu.Item>
                        </Menu.Items>
                      </Transition>
                    </Menu>
                  ) : null}
                </div>
              </div>
            </div>
            <Disclosure.Panel className="sm:hidden">
              <div className="px-2 pt-2 pb-3 space-y-1">
                {navigation.map((item) => (
                  <NavLink
                    key={item.name}
                    to={item.href}
                    className={({ isActive }) => {
                      return (
                        "block px-3 py-2 rounded-md text-base font-medium no-underline " +
                        (!isActive
                          ? " text-gray-300 hover:bg-purple-900 hover:text-white"
                          : "bg-purple-900 text-white")
                      );
                    }}
                  >
                    {item.name}
                  </NavLink>
                ))}
                {loggedIn ? (
                  <>
                    <NavLink
                      to={"/chat"}
                      className="block px-3 py-2 rounded-md text-base font-medium no-underline text-gray-300 hover:bg-purple-900 hover:text-white"
                    >
                      Chat
                    </NavLink>
                    <NavLink
                      to={"/projects"}
                      className="block px-3 py-2 rounded-md text-base font-medium no-underline text-gray-300 hover:bg-purple-900 hover:text-white"
                    >
                      Projects
                    </NavLink>
                    {isAdmin ? (
                      <>
                        <NavLink
                          to={"/invalidUsers"}
                          className="block px-3 py-2 rounded-md text-base font-medium no-underline text-gray-300 hover:bg-purple-900 hover:text-white"
                        >
                          Requests
                        </NavLink>
                        <NavLink
                          to={"/users"}
                          className="block px-3 py-2 rounded-md text-base font-medium no-underline text-gray-300 hover:bg-purple-900 hover:text-white"
                        >
                          Users
                        </NavLink>
                      </>
                    ) : (
                      <NavLink
                        to={"/tasks"}
                        className="block px-3 py-2 rounded-md text-base font-medium no-underline text-gray-300 hover:bg-purple-900 hover:text-white"
                      >
                        Tasks
                      </NavLink>
                    )}
                  </>
                ) : (
                  <>
                    <NavLink
                      to={"/login"}
                      className="block px-3 py-2 rounded-md text-base font-medium no-underline text-gray-300 hover:bg-purple-900 hover:text-white"
                    >
                      Login
                    </NavLink>
                    <NavLink
                      to={"/register"}
                      className="block px-3 py-2 rounded-md text-base font-medium no-underline text-gray-300 hover:bg-purple-900 hover:text-white"
                    >
                      Register
                    </NavLink>
                  </>
                )}
              </div>
            </Disclosure.Panel>
          </>
        )}
      </Disclosure>
      <div className="bg-orange-50">
        <div className="max-w-7xl mx-auto min-h-screen px-3 py-2">
          {props.children}
        </div>
      </div>
    </>
  );
}
