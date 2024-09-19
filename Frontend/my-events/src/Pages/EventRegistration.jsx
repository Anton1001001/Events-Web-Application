import React, { useState, useEffect } from 'react';
import { Box, Button, Input, Stack, Text, Heading, FormControl, FormLabel } from '@chakra-ui/react';
import axios from 'axios';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { getUserId } from '../authUtils';
import config from '../config';

function EventRegistration() {
  const location = useLocation();
  const { user, eventId } = location.state || {}; 
  const [event, setEvent] = useState(null);
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [dateOfBirth, setDateOfBirth] = useState('');
  const [error, setError] = useState(null);
  const [registrationResult, setRegistrationResult] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchEvent = async () => {
      try {
        const response = await axios.get(`${config.serverUrl}/event/${eventId}`, {
          withCredentials: true
        });
        setEvent(response.data);
      } catch (error) {
        console.error("Failed to load event details:", error);
      }
    };

    fetchEvent();
  }, [eventId]);

  useEffect(() => {
    if (user) {
      const formattedDateOfBirth = user.dateOfBirth
        ? new Date(user.dateOfBirth).toISOString().split('T')[0]
        : '';
      setFirstName(user.firstName || '');
      setLastName(user.lastName || '');
      setDateOfBirth(formattedDateOfBirth);
    }
  }, [user]);

  const handleSubmit = async (event) => {
    event.preventDefault();
    
    const userId = getUserId();

    if (!userId) {
      setError('User is not authenticated.');
      return;
    }

    try {
      const response = await axios.post(`${config.serverUrl}/user/register`, {
        userId,
        eventId,
        firstName,
        lastName,
        dateOfBirth
      }, {
        withCredentials: true
      });

      setRegistrationResult(response.data);

      navigate(`/event/${eventId}`);
    } catch (error) {
      setError('Registration failed. Please try again.');
      console.error("Failed to register:", error);
    }
  };

  if (!event) return <Text>Loading...</Text>;

  return (
    <Box p={4}>
      <Heading mb={4}>Register for {event.name}</Heading>
      <Stack spacing={4}>
        <Text mb={2}><strong>Description:</strong> {event.description}</Text>
        <Text mb={2}><strong>Date & Time:</strong> {new Date(event.dateTime).toLocaleDateString() + ' ' +
    new Date(event.dateTime).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</Text>
        <Text mb={2}><strong>Location:</strong> {event.location}</Text>
        <Text mb={2}><strong>Category:</strong> {event.category}</Text>
      </Stack>
      <form onSubmit={handleSubmit}>
        <Stack spacing={4} mt={4}>
          <FormControl id="firstName" isRequired>
            <FormLabel>First Name</FormLabel>
            <Input
              placeholder="First Name"
              value={firstName}
              onChange={(e) => setFirstName(e.target.value)}
            />
          </FormControl>
          <FormControl id="lastName" isRequired>
            <FormLabel>Last Name</FormLabel>
            <Input
              placeholder="Last Name"
              value={lastName}
              onChange={(e) => setLastName(e.target.value)}
            />
          </FormControl>
          <FormControl id="dateOfBirth" isRequired>
            <FormLabel>Date of Birth</FormLabel>
            <Input
              placeholder="Date of Birth"
              type="date"
              value={dateOfBirth}
              onChange={(e) => setDateOfBirth(e.target.value)}
            />
          </FormControl>
          {error && <Text color="red.500">{error}</Text>}
          <Button type="submit" colorScheme="blue">Register</Button>
        </Stack>
      </form>
    </Box>
  );
}

export default EventRegistration;
