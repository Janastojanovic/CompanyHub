export default function Home() {
  return (
    <div>
      <div className="w-screen h-screen overflow-hidden relative before:block before:absolute before:bg-black before:h-full before:w-full before:top-0 before:left-0 before:z-10 before:opacity-30">
        <img
          src="https://images.pexels.com/photos/7199145/pexels-photo-7199145.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"
          className="absolute top-0 left-0 w-screen min-h-full object-cover"
          alt=""
        />
        <div className="relative z-20 max-w-screen-lg mx-auto grid grid-cols-12 h-full items-center">
          <div className="col-span-6">
            <span className="uppercase text-white text-xs font-bold mb-2 block">
              WE ARE EXPERTS
            </span>
            <h1 className="text-white font-extrabold text-5xl mb-8">
              Welcome to CompanyHub
            </h1>
            <p className="text-stone-100 text-base">
              CompanyHub is your premier solution for organizing and connecting
              remote teams. Our platform streamlines communication, fosters
              collaboration, and enhances productivity for distributed teams,
              regardless of location.
            </p>
          </div>
        </div>
      </div>
      <div className="bg-[#f7d0b6] py-20">
        <div className="max-w-screen-lg mx-auto flex justify-between items-center">
          <div className="max-w-xl">
            <h2 className="font-black text-sky-950 text-3xl mb-4">
              Connect and Collaborate Globally
            </h2>
            <p className="text-base text-sky-950">
              Join the ranks of leading organizations in over 90 countries who
              trust CompanyHub for seamless communication and collaboration.
              Empower your team to excel with our intuitive platform designed
              for the modern remote workforce.
            </p>
          </div>
        </div>
      </div>
      <div className="py-12 relative overflow-hidden bg-white">
        <div className="grid grid-cols-2 max-w-screen-lg mx-auto">
          <div className="w-full flex flex-col items-end pr-16">
            <h2 className="text-[#64618C] font-bold text-2xl max-w-xs text-right mb-12 mt-10">
              Elevate Your Team's Efficiency
            </h2>
            <div className="h-full mt-auto overflow-hidden relative">
              <img
                src="https://images.pexels.com/photos/459653/pexels-photo-459653.jpeg"
                className="h-full w-full object-contain"
                alt=""
              />
            </div>
          </div>
          <div className="py-20 bg-purple-200 relative before:absolute before:h-full before:w-screen before:bg-sky-950 before:top-0 before:left-0">
            <div className="relative z-20 pl-12">
              <h2 className="text-[#f7d0b6] font-black text-5xl leading-snug mb-10">
                Empowering Remote Collaboration
              </h2>
              <p className="text-white text-sm">
                CompanyHub provides the tools your team needs to collaborate
                effectively from anywhere. From real-time chat to task
                management, our platform ensures seamless communication and
                coordination.
              </p>
            </div>
          </div>
        </div>
      </div>
      <div className="py-4 relative overflow-hidden bg-white">
        <div className="grid grid-cols-2 max-w-screen-lg mx-auto">
          <div className="py-20 bg-slate-100 relative before:absolute before:h-full before:w-screen before:bg-[#f7d0b6] before:top-0 before:right-0">
            <div className="relative z-20 pl-12">
              <h2 className="text-sky-950 font-black text-5xl leading-snug mb-10">
                Seamless Team Collaboration
              </h2>
              <p className="text-sky-950 text-sm">
                Break down barriers and facilitate collaboration with
                CompanyHub. Our platform enables smooth communication, efficient
                task management, and enhanced team productivity, no matter where
                your team members are located.
              </p>
            </div>
          </div>
          <div className="w-full flex flex-col pl-16">
            <h2 className="text-[#64618C] font-bold text-2xl max-w-xs text-left mb-12 mt-10">
              Simplifying Remote Work
            </h2>
            <div className="h-full mt-auto overflow-hidden relative">
              <img
                src="https://images.pexels.com/photos/12944685/pexels-photo-12944685.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"
                className="h-full w-full object-contain"
                alt=""
              />
            </div>
          </div>
        </div>
      </div>
      <div className="py-12 relative overflow-hidden bg-white">
        <div className="grid grid-cols-2 max-w-screen-lg mx-auto">
          <div className="w-full flex flex-col items-end pr-16">
            <h2 className="text-[#64618C] font-bold text-2xl max-w-xs text-right mb-12 mt-10">
              Achieve More Together
            </h2>
            <div className="h-full mt-auto overflow-hidden relative">
              <img
                src="https://images.pexels.com/photos/3811593/pexels-photo-3811593.jpeg"
                className="h-full w-full object-contain"
                alt=""
              />
            </div>
          </div>
          <div className="py-20 bg-slate-100 relative before:absolute before:h-full before:w-screen before:bg-sky-950 before:top-0 before:left-0">
            <div className="relative z-20 pl-12">
              <h2 className="text-[#f7d0b6] font-black text-5xl leading-snug mb-10">
                Unlock Your Team's Potential
              </h2>
              <p className="text-white text-sm">
                With CompanyHub, your team can unleash its full potential. Our
                platform empowers collaboration, streamlines communication, and
                boosts productivity, allowing your organization to thrive in
                today's remote work environment.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
