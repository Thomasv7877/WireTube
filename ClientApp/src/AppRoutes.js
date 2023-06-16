import { YoutubeApp } from "./components/YoutubeApp";
import { About } from "./components/About";
import MusicPlayerInReact from "./components/MusicPlayerInReact";

const AppRoutes = [
  {
    path: '/',
    element: <YoutubeApp />
  },
  {
    index: true,
    path: '/YoutubeApp',
    element: <YoutubeApp />
  },
  {
    path: '/YoutubePlay',
    element: <MusicPlayerInReact />
  },
  {
    path: '/About',
    element: <About />
  }
];

export default AppRoutes;
