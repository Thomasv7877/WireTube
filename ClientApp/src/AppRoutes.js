import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
//import { Home } from "./components/Home";
import { GenXml } from "./components/GenXml";
import { Login} from "./components/Login";
import { YoutubeApp } from "./components/YoutubeApp";
import { YoutubePlay } from "./components/YoutubePlay";
import { About } from "./components/About";

const AppRoutes = [
  {
    path: '/',
    element: <YoutubeApp />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/fetch-data',
    element: <FetchData />
  },
  {
    path: '/gen-xml',
    requireAuth: true,
    element: <GenXml />
  },
  {
    path: '/Login',
    element: <Login />
  },
  {
    index: true,
    path: '/YoutubeApp',
    element: <YoutubeApp />
  },
  {
    path: '/YoutubePlay',
    element: <YoutubePlay />
  },
  {
    path: '/About',
    element: <About />
  }
];

export default AppRoutes;
