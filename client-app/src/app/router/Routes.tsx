import { createBrowserRouter, Navigate, RouteObject } from 'react-router-dom';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import ActivityDetails from '../../features/activities/details/ActivityDetails';
import ActivityForm from '../../features/activities/form/activityForm';
import NotFound from '../../features/errors/NotFound';
import ServerError from '../../features/errors/ServerError';
import TestErrors from '../../features/errors/TestError';
import ProfilePage from '../../features/profiles/ProfilePage';
import App from '../layout/App';
import RequireAuth from './RequireAuth';

export const routes: RouteObject[] = [
  {
    path: '/',
    element: <App />, //top of tree
    children: [
      {
        element: <RequireAuth />, // protect routes from unauthorized users
        children: [
          { path: 'activities', element: <ActivityDashboard /> },
          { path: 'activities/:id', element: <ActivityDetails /> },
          { path: 'createActivity', element: <ActivityForm key="create" /> },
          { path: 'manage/:id', element: <ActivityForm key="manage" /> },
          { path: 'profiles/:username', element: <ProfilePage /> },         
          { path: 'errors', element: <TestErrors /> },
        ],
      },

      { path: 'not-found', element: <NotFound /> },
      { path: 'server-error', element: <ServerError /> },
      { path: '*', element: <Navigate replace to="/not-found" /> },
    ],
  },
];

export const router = createBrowserRouter(routes);

// keys will make sure that if we navigate from edit to create activity the form will be reseted, although this is the same component
