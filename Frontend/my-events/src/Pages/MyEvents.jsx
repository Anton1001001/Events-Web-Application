import React, { useState, useEffect } from 'react';
import { Box, Heading, Text, Stack } from '@chakra-ui/react';
import axios from 'axios';
import { getUserId } from '../authUtils';
import EventCard from '../Components/EventCard';
import config from '../config';

const MyEvents = () => {
  const [events, setEvents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(true);

  useEffect(() => {
    const fetchUserEvents = async () => {
      try {
        const userId = getUserId();

        if (!userId) {
          setIsAuthenticated(false);
          setLoading(false);
          return;
        }

        const response = await axios.get(`${config.serverUrl}/user/events`, {
          params: { userId },
          withCredentials: true
        });

        setEvents(response.data);
      } catch (error) {
        setError('Failed to load events. Please try again.');
        console.error('Error fetching user events:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchUserEvents();
  }, []);

  if (loading) return <Text>Loading...</Text>;
  if (!isAuthenticated) return <Text p={4} fontSize="xl" color="gray.500">Please authenticate to view your events</Text>;
  if (error) return <Text color="red.500">{error}</Text>;

  return (
    <Box p={4}>
      <Heading mb={4}>My Events</Heading>
      <Box
        display="flex" 
        flexWrap="wrap" 
        justifyContent="center"
        gap={4}
      >
        {events.length === 0 ? (
          <Text fontSize="xl" color="gray.500">You are not registered for any event</Text>
        ) : (
          events.map((event) => (
            <EventCard key={event.id} event={event} />
          ))
        )}
      </Box>
    </Box>
  );
};

export default MyEvents;
