import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";
import { GenXml } from "./components/GenXml";
import { Login} from "./components/Login";

const AppRoutes = [
  {
    index: true,
    element: <Home />
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
  }
];

export default AppRoutes;
