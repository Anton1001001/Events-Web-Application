import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Flex, Box, Text, Button, Spacer, ButtonGroup } from '@chakra-ui/react';
import axios from 'axios';
import { useUserRole } from '../hooks/useUserRole';
import config from '../config';

const Header = () => {
  const navigate = useNavigate();
  const role = useUserRole();

  const handleLogout = async () => {
    try {
      await axios.post(`${config.serverUrl}/auth/logout`, {}, { withCredentials: true });
      console.log('Logout successful');
      navigate('/auth');
    } catch (error) {
      console.error('Error during logout:', error);
    }
  };

  return (
    <Flex as="header" p="4" bg="teal.500" color="white" align="center" boxShadow="md">
      <Box>
        <Text fontSize="2xl" fontWeight="bold" letterSpacing="wide">
          Event Management
        </Text>
      </Box>
      <Spacer />
      <ButtonGroup spacing="6">
        <Link to="/events">
          <Button variant="outline" colorScheme="white" _hover={{ bg: 'whiteAlpha.300' }}>
            Events List
          </Button>
        </Link>
        <Link to="/auth">
          <Button variant="outline" colorScheme="white" _hover={{ bg: 'whiteAlpha.300' }}>
            Login/Register
          </Button>
        </Link>
        <Link to="/my-events">
          <Button variant="outline" colorScheme="white" _hover={{ bg: 'whiteAlpha.300' }}>
            My Events
          </Button>
        </Link>
          <Link to="/admin">
            <Button variant="solid" colorScheme="blue" _hover={{ bg: 'blue.600' }}>
              Admin
            </Button>
          </Link>
        <Button variant="solid" colorScheme="red" _hover={{ bg: 'red.600' }} onClick={handleLogout}>
          Logout
        </Button>
      </ButtonGroup>
    </Flex>
  );
};

export default Header;
