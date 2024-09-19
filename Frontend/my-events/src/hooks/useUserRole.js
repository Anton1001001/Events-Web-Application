import { useState, useEffect } from 'react';
import { getUserRole } from '../authUtils';

export function useUserRole() {
  const [role, setRole] = useState(null);

  useEffect(() => {
    const fetchRole = async () => {
      try {
        const userRole = await getUserRole();
        setRole(userRole);
      } catch (error) {
        console.error('Error fetching user role:', error);
      }
    };

    fetchRole();
  }, []);

  return role;
}
