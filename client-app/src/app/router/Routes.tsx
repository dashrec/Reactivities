import { createBrowserRouter, RouteObject } from 'react-router-dom';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import ActivityDetails from '../../features/activities/details/ActivityDetails';
import ActivityForm from '../../features/activities/form/activityForm';
import App from '../layout/App';

export const routes: RouteObject[] = [
  {
    path: '/',
    element: <App />, //top of tree
    children: [
      { path: 'activities', element: <ActivityDashboard /> },
      { path: 'activities/:id', element: <ActivityDetails /> },
      { path: 'createActivity', element: <ActivityForm key="create" /> },
      { path: 'manage/:id', element: <ActivityForm key="manage" /> },
    ],
  },
];

export const router = createBrowserRouter(routes);

// keys will make sure that if we navigate from edit to create activity the form will be reseted, although this is the same component
