import ApproveUser from "./ApproveUser";

function User(props) {
  return (
    <div className="m-3 py-8 px-8 max-w-sm bg-white rounded-xl shadow-lg space-y-2 sm:py-4 sm:flex sm:items-center sm:space-y-0 sm:space-x-6">
      <img
        className="object-cover rounded-full h-[100px] w-[100px] block mx-auto h-24 rounded-full sm:mx-0 sm:shrink-0"
        src={props.img}
        alt=""
      />
      <div className="text-center space-y-2 sm:text-left">
        <div className="space-y-0.5">
          <p className="text-lg text-black font-semibold">
            {props.firstname} {props.lastname}
          </p>
          {props.invalid ? (
            <>
              <p className="text-slate-500 font-medium">
                Contact: {props.email}
              </p>
              <p className="text-slate-500 font-medium">
                {props.phone}
              </p>
            </>
          ) : 
          (
            <>
            <p className="text-slate-500 font-medium">
                Email: {props.email}
              </p>
              <p className="text-slate-500 font-medium">
                Phone: {props.phone}
              </p>
              <p className="text-slate-500 font-medium">
                Username: {props.username}
              </p>
            </>
          )}

          {/* <p className="text-slate-500 font-medium">
            Current budget: {props.currentBudget}
          </p> */}
          {/* <div className="container">
        <div className="progressbar-container">
          <div className="progressbar-complete" style={{width: `${props.procentage}%`}}>
            <div className="progressbar-liquid"></div>
          </div>
        </div>
      </div> */}
        </div>
        {props.deleteInvalidUser}
        {props.invalid ? <ApproveUser id={props.id} /> : null}
      </div>
    </div>
  );
}
export default User;
