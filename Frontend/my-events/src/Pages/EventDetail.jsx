import React, { useEffect, useState } from 'react';
import { Box, Heading, Text, Image, Flex, Button } from '@chakra-ui/react';
import axios from 'axios';
import { useParams, useNavigate } from 'react-router-dom';
import { getUserId } from '../authUtils';
import config from '../config';

function EventDetail() {
  const { id: eventId } = useParams();
  const [event, setEvent] = useState(null);
  const [isRegistered, setIsRegistered] = useState(false);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    const checkAuthentication = () => {
      const userId = getUserId();
      setIsAuthenticated(!!userId);
    };

    checkAuthentication();

    const fetchEvent = async () => {
      try {
        const response = await axios.get(`${config.serverUrl}/event/${eventId}`, {
          withCredentials: true
        });
        setEvent(response.data);
      } catch (error) {
        console.error('Failed to load event details:', error);
      }
    };

    fetchEvent();
  }, [eventId]);

  useEffect(() => {
    const checkRegistration = async () => {
      try {
        const userId = getUserId();
        if (userId) {
          const response = await axios.get(`${config.serverUrl}/user/check-registration`, {
            params: { userId, eventId },
            withCredentials: true
          });
          setIsRegistered(response.data);
        }
      } catch (error) {
        console.error('Failed to check registration status:', error);
      }
    };

    checkRegistration();
  }, [eventId]);

  const handleRegister = async () => {
    if (!isAuthenticated) {
      alert('User is not authenticated.');
      return;
    }

    const userId = getUserId();
    if (!userId) {
      alert('User is not authenticated.');
      return;
    }

    const userResponse = await axios.get(`${config.serverUrl}/user/${userId}`, {
      withCredentials: true
    });

    const user = {
      firstName: userResponse.data.firstName,
      lastName: userResponse.data.lastName,
      dateOfBirth: userResponse.data.dateOfBirth
    };

    navigate(`/event/${eventId}/register`, {
      state: { user, eventId }
    });
  };

  const handleCancelRegistration = async () => {
    try {
      if (!isAuthenticated) {
        alert('User is not authenticated.');
        return;
      }

      const userId = getUserId();
      if (!userId) {
        alert('User is not authenticated.');
        return;
      }

      await axios.post(`${config.serverUrl}/user/cancel-registration`, {
        userId,
        eventId
      }, {
        withCredentials: true
      });

      setIsRegistered(false);
      const response = await axios.get(`${config.serverUrl}/event/${eventId}`, { withCredentials: true });
      setEvent(response.data);
    } catch (error) {
      console.error('Failed to cancel registration:', error);
    }
  };

  if (!event) return <Text>Loading...</Text>;

  const formattedDateTime = new Date(event.dateTime).toLocaleDateString() + ' ' +
    new Date(event.dateTime).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

  return (
    <Box p={4}>
      <Image
        src={`${config.serverUrl}${event.imageUrl}`}
        alt={event.name}
        borderRadius="lg"
        mb={4}
      />
      <Heading mb={4}>{event.name}</Heading>
      <Text mb={2}><strong>Description:</strong> {event.description}</Text>
      <Text mb={2}><strong>Date & Time:</strong> {formattedDateTime}</Text>
      <Text mb={2}><strong>Location:</strong> {event.location}</Text>
      <Text mb={2}><strong>Category:</strong> {event.category}</Text>
      <Flex mb={2} alignItems="center">
        <Text fontWeight="bold">Available Seats:</Text>
        <Box ml={2}>
          {event.availableSeats > 0 ? (
            <Text>{event.availableSeats}</Text>
          ) : (
            <Text color="red.500">No seats available</Text>
          )}
        </Box>
      </Flex>
      {isRegistered ? (
        <Button colorScheme="red" onClick={handleCancelRegistration}>
          Cancel Registration
        </Button>
      ) : (
        <Button
          colorScheme="blue"
          onClick={handleRegister}
          isDisabled={!isAuthenticated}
        >
          Register
        </Button>
      )}
    </Box>
  );
}

export default EventDetail;
