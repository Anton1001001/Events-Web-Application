import { jwtDecode } from 'jwt-decode';
import Cookies from 'js-cookie';

export function getUserRole() {
  const token = Cookies.get('jwt');
  if (!token) return null;

  try {
    const decoded = jwtDecode(token);
    return decoded.role;
  } catch (error) {
    console.error('Failed to decode JWT:', error);
    return null;
  }
}

export function isAdmin() {
  return getUserRole() === 'Admin';
}

export function getUserId() {
    const token = Cookies.get('jwt');
    if (!token) return null;
  
    try {
      const decoded = jwtDecode(token);
      return decoded.userId;  
    } catch (error) {
      console.error('Failed to decode JWT:', error);
      return null;
    }
  }