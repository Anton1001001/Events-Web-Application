import React, { useState, useEffect } from 'react';
import { Box, Text, Heading, useToast, Card, CardBody, CardHeader, Divider, Stack, useColorModeValue, Flex } from '@chakra-ui/react';
import axios from 'axios';
import { useParams } from 'react-router-dom';
import config from '../config';

const EventUsersPage = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const { eventId } = useParams();
  const toast = useToast();

  const cardBgColor = useColorModeValue('white', 'gray.800');
  const cardBorderColor = useColorModeValue('gray.200', 'gray.700');

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const response = await axios.get(`${config.serverUrl}/event/${eventId}/users`, { withCredentials: true });
        console.log('API Response:', response.data);
        setUsers(response.data);
      } catch (error) {
        console.error('Failed to load users:', error);
        toast({ title: 'Error', description: 'Failed to load users.', status: 'error' });
      } finally {
        setLoading(false);
      }
    };

    fetchUsers();
  }, [eventId, toast]);

  if (loading) {
    return <Text>Loading...</Text>;
  }

  return (
    <Box p={4}>
      <Heading mb={6}>Users for Event</Heading>
      <Flex wrap="wrap" spacing={4} justify="center">
        {users.length === 0 ? (
          <Text>No users found for this event.</Text>
        ) : (
          users.map(user => (
            <Card
              key={user.email}
              width="300px"
              bg={cardBgColor}
              boxShadow="lg"
              borderWidth="1px"
              borderColor={cardBorderColor}
              borderRadius="md"
              overflow="hidden"
              m={2}
            >
              <CardHeader bg={useColorModeValue('blue.100', 'blue.700')} p={4}>
                <Heading size="md" color={useColorModeValue('blue.800', 'blue.100')}>
                  {user.firstName} {user.lastName}
                </Heading>
              </CardHeader>
              <Divider />
              <CardBody p={4}>
                <Stack spacing={3}>
                  <Text color={useColorModeValue('gray.700', 'gray.300')}>
                    Date of Birth: {user.dateOfBirth ? new Date(user.dateOfBirth).toLocaleDateString() : 'N/A'}
                  </Text>
                  <Text color={useColorModeValue('gray.700', 'gray.300')}>
                    Registered on: {user.registerDate ? `${new Date(user.registerDate).toLocaleDateString()} - ${new Date(user.registerDate).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}` : 'N/A'}
                  </Text>
                  <Text color={useColorModeValue('gray.700', 'gray.300')}>Email: {user.email}</Text>
                </Stack>
              </CardBody>
            </Card>
          ))
        )}
      </Flex>
    </Box>
  );
};

export default EventUsersPage;
