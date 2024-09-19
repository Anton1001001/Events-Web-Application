import React, { useState } from 'react';
import { Box, Button, Input, FormControl, FormLabel, Text, Stack, Heading, useToast } from '@chakra-ui/react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom'; 

import config from '../config';
import { useUserRole } from '../hooks/useUserRole';

const AuthPage = () => {
  const [isLogin, setIsLogin] = useState(true);
  const [formData, setFormData] = useState({ email: '', password: '' });
  const navigate = useNavigate();
  const toast = useToast();
  const role = useUserRole();

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (isLogin) {
        const response = await axios.post(`${config.serverUrl}/auth/login`, {
          email: formData.email,
          password: formData.password,
        }, {
          withCredentials: true,
        });

        console.log('Login successful');
        navigate('/events');
      } else {
        const response = await axios.post(`${config.serverUrl}/auth/register`, {
          email: formData.email,
          password: formData.password,
        });

        if (response.status === 200) {
          console.log('Registration successful');
          toast({
            title: "Registration Successful",
            description: "You have successfully registered. You can now log in.",
            status: "success",
            duration: 5000,
            isClosable: true,
          });
          setIsLogin(true);
        }
      }
    } catch (error) {
      if (error.response) {
        if (error.response.status === 400) {
          if (isLogin) {
            toast({
              title: "Login Error",
              description: error.response.data || "Invalid login or password.",
              status: "error",
              duration: 5000,
              isClosable: true,
            });
          } else {
            toast({
              title: "Registration Error",
              description: error.response.data || "Registration failed.",
              status: "error",
              duration: 5000,
              isClosable: true,
            });
          }
        } else {
          toast({
            title: "An error occurred",
            description: error.response.data || error.message,
            status: "error",
            duration: 5000,
            isClosable: true,
          });
        }
      } else {
        toast({
          title: "An error occurred",
          description: error.message,
          status: "error",
          duration: 5000,
          isClosable: true,
        });
      }
      console.error('Error:', error.response ? error.response.data : error.message);
    }
  };

  return (
    <Box maxW="md" mx="auto" mt={8} p={4} borderWidth={1} borderRadius="lg">
      <Heading textAlign="center" mb={6}>
        {isLogin ? 'Login' : 'Register'}
      </Heading>
      <form onSubmit={handleSubmit}>
        <Stack spacing={4}>
          <FormControl>
            <FormLabel>Email</FormLabel>
            <Input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleInputChange}
              required
            />
          </FormControl>
          <FormControl>
            <FormLabel>Password</FormLabel>
            <Input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleInputChange}
              required
            />
          </FormControl>
          <Button colorScheme="teal" type="submit">
            {isLogin ? 'Login' : 'Register'}
          </Button>
        </Stack>
      </form>
      <Text mt={4} textAlign="center">
        {isLogin ? "Don't have an account?" : 'Already have an account?'}
        <Button variant="link" colorScheme="teal" onClick={() => setIsLogin(!isLogin)}>
          {isLogin ? 'Register' : 'Login'}
        </Button>
      </Text>
    </Box>
  );
};

export default AuthPage;
