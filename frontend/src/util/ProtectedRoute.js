import { Navigate, Outlet } from 'react-router-dom';

const ProtectedRoute = ({ children }) => {
  const Auth = localStorage.getItem('user-token');

  console.log(Auth);
  if (!Auth) {
    console.log('redirect');
    return <Navigate to="/login" />;
  }
  if (Auth) {
    console.log('children');
    return <Outlet />;
  }
};

export default ProtectedRoute;
