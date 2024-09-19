import React, { useState, useEffect } from 'react';
import { Box, Button, FormControl, FormLabel, Input, Stack, Text, VStack, Heading, useToast } from '@chakra-ui/react';
import axios from 'axios';
import { isAdmin } from '../authUtils';
import { useNavigate } from 'react-router-dom';
import config from '../config';

const AdminPage = () => {
  const [events, setEvents] = useState([]);
  const [selectedEvent, setSelectedEvent] = useState(null);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    dateTime: '',
    location: '',
    category: '',
    maxUsers: '',
    image: null,
  });
  const [isUserAdmin, setIsUserAdmin] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const pageSize = 5;
  const toast = useToast();
  const navigate = useNavigate();

  useEffect(() => {
    setIsUserAdmin(isAdmin());

    if (!isUserAdmin) return;

    const fetchEvents = async (page) => {
      try {
        const response = await axios.get(`${config.serverUrl}/event`, {
          params: {
            page: page,
            pageSize: pageSize,
          },
          withCredentials: true,
        });
        setEvents(response.data.items || []);
        setTotalPages(response.data.totalPages || 1);
      } catch (error) {
        console.error('Failed to load events:', error);
      }
    };

    fetchEvents(currentPage);
  }, [isUserAdmin, currentPage]);

  if (!isUserAdmin) {
    return <Box p={4}><Text>You do not have permission to view this page.</Text></Box>;
  }

  const handleInputChange = (e) => {
    const { name, value, files } = e.target;
    setFormData({
      ...formData,
      [name]: name === "maxUsers" ? parseInt(value) : value,
      image: files ? files[0] : formData.image,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!selectedEvent && !formData.image) {
      toast({ title: 'Error', description: 'Please select an image for the new event.', status: 'error' });
      return;
    }

    try {
      const form = new FormData();

      for (const key in formData) {
        if (key === 'image') {
          if (formData.image && typeof formData.image !== 'string') {
            form.append('image', formData.image);
          } else {
            if (!selectedEvent) {
              toast({ title: 'Error', description: 'Please select an image for the new event.', status: 'error' });
              return;
            }
            form.append('image', null);
          }
        } else {
          form.append(key, formData[key]);
        }
      }
  
      if (selectedEvent) {
        form.append('imageUrl', selectedEvent.imageUrl || '');
      }
  
      if (selectedEvent) {
        await axios.put(`${config.serverUrl}/event/${selectedEvent.id}`, form, {
          headers: { 'Content-Type': 'multipart/form-data' },
          withCredentials: true,
        });
        toast({ title: 'Event updated successfully', status: 'success' });
      } else {
        await axios.post(`${config.serverUrl}/event`, form, {
          headers: { 'Content-Type': 'multipart/form-data' },
          withCredentials: true,
        });
        toast({ title: 'Event created successfully', status: 'success' });
      }

      setFormData({
        name: '',
        description: '',
        dateTime: '',
        location: '',
        category: '',
        maxUsers: '',
        image: null,
      });
      setSelectedEvent(null);
    } catch (error) {
      console.error('Error:', error.response ? error.response.data : error.message);
      toast({ title: 'Error', description: 'An error occurred while saving the event.', status: 'error' });
    }
  };

  const handleEdit = (event) => {
    setSelectedEvent(event);
    setFormData({
      name: event.name,
      description: event.description,
      dateTime: event.dateTime,
      location: event.location,
      category: event.category,
      maxUsers: event.maxUsers,
      image: null,
    });
  };

  const handleDelete = async (id) => {
    try {
      await axios.delete(`${config.serverUrl}/event/${id}`, { withCredentials: true });
      setEvents(events.filter(event => event.id !== id));
      toast({ title: 'Event deleted successfully', status: 'success' });
    } catch (error) {
      console.error('Error:', error.response ? error.response.data : error.message);
      toast({ title: 'Error', description: 'An error occurred while deleting the event.', status: 'error' });
    }
  };

  const handleShowUsers = (eventId) => {
    navigate(`/event/${eventId}/users`);
  };

  const handleNextPage = () => {
    if (currentPage < totalPages) {
      setCurrentPage(currentPage + 1);
    }
  };

  const handlePrevPage = () => {
    if (currentPage > 1) {
      setCurrentPage(currentPage - 1);
    }
  };

  return (
    <Box p={4}>
      <Heading mb={6}>Admin - Manage Events</Heading>
      <form onSubmit={handleSubmit}>
        <VStack spacing={4} align="stretch">
          <FormControl>
            <FormLabel>Event Name</FormLabel>
            <Input type="text" name="name" value={formData.name} onChange={handleInputChange} required />
          </FormControl>
          <FormControl>
            <FormLabel>Description</FormLabel>
            <Input type="text" name="description" value={formData.description} onChange={handleInputChange} required />
          </FormControl>
          <FormControl>
            <FormLabel>Date and Time</FormLabel>
            <Input type="datetime-local" name="dateTime" value={formData.dateTime} onChange={handleInputChange} required />
          </FormControl>
          <FormControl>
            <FormLabel>Location</FormLabel>
            <Input type="text" name="location" value={formData.location} onChange={handleInputChange} required />
          </FormControl>
          <FormControl>
            <FormLabel>Category</FormLabel>
            <Input type="text" name="category" value={formData.category} onChange={handleInputChange} required />
          </FormControl>
          <FormControl>
            <FormLabel>Max Participants</FormLabel>
            <Input type="number" name="maxUsers" value={formData.maxUsers} onChange={handleInputChange} required />
          </FormControl>
          <FormControl>
            <FormLabel>Image</FormLabel>
            <Input type="file" name="image" onChange={handleInputChange} />
          </FormControl>
          <Button type="submit" colorScheme="teal">
            {selectedEvent ? 'Update Event' : 'Create Event'}
          </Button>
        </VStack>
      </form>

      <Box mt={8}>
        <Heading size="md" mb={4}>Existing Events</Heading>
        {events.map(event => (
          <Box key={event.id} mb={4} p={4} borderWidth={1} borderRadius="md">
            <Text fontSize="xl">{event.name}</Text>
            <Text>{event.description}</Text>
            <Text>{new Date(event.dateTime).toLocaleDateString()} - {event.location}</Text>
            <Button mt={2} colorScheme="blue" onClick={() => handleEdit(event)}>
              Edit
            </Button>
            <Button mt={2} ml={2} colorScheme="red" onClick={() => handleDelete(event.id)}>
              Delete
            </Button>
            <Button mt={2} ml={2} colorScheme="purple" onClick={() => handleShowUsers(event.id)}>
              Show Users
            </Button>
          </Box>
        ))}

      {/* Pagination controls */}
      <Box display="flex" justifyContent="center" mt={4}>
        <Button onClick={handlePrevPage} isDisabled={currentPage === 1} mr={2}>
          Previous
        </Button>
        <Text>{currentPage} of {totalPages}</Text>
        <Button onClick={handleNextPage} isDisabled={currentPage === totalPages} ml={2}>
          Next
        </Button>
      </Box>
      </Box>
    </Box>
  );
};

export default AdminPage;
