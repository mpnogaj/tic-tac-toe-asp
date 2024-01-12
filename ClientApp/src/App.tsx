import React from 'react';
import { Navigate, RouterProvider, createBrowserRouter } from 'react-router-dom';

import LoadingComponent from './components/LoadingComponent';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import ProfilePage from './pages/ProfilePage';
import RegisterPage from './pages/RegisterPage';
import RoomPage from './pages/RoomPage';
import RoomsPage from './pages/RoomsPage';
import { isAuth, isPlayer } from './utils/auth';

const router = createBrowserRouter([
	{
		path: '/',
		element: <Navigate to="/rooms" />
	},
	{
		path: '/login',
		element: <LoginPage />
	},
	{
		path: '/register',
		element: <RegisterPage />
	},
	{
		path: '/rooms',
		element: (
			<ProtectedRoute
				child={<RoomsPage />}
				loading={<LoadingComponent />}
				fallback={<Navigate to="/login" />}
				checkAuthFunc={isPlayer}
			/>
		)
	},
	{
		path: '/room/:roomGuid',
		element: (
			<ProtectedRoute
				child={<RoomPage />}
				loading={<LoadingComponent />}
				fallback={<Navigate to="/login" />}
				checkAuthFunc={isPlayer}
			/>
		)
	},
	{
		path: '/profile',
		element: (
			<ProtectedRoute
				child={<ProfilePage />}
				loading={<LoadingComponent />}
				fallback={<Navigate to="/login" />}
				checkAuthFunc={isAuth}
			/>
		)
	}
]);

const App: React.FC = () => <RouterProvider router={router} />;

export default App;
