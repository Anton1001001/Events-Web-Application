import React, { useEffect, useState } from 'react';
import { Box, Text, Button } from '@chakra-ui/react';
import axios from 'axios';

import EventSearchFilter from '../Components/EventSearchFilter';
import config from '../config';
import EventCard from '../Components/EventCard';

function EventList() {
  const [events, setEvents] = useState([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const pageSize = 5;

  const [searchParams, setSearchParams] = useState({});

  const fetchEvents = async (page, searchParams = {}) => {
    try {
      const response = await axios.get(`${config.serverUrl}/event`, {
        params: {
          page: page,
          pageSize: pageSize,
          title: searchParams.title || '',
          date: searchParams.date || '',
          category: searchParams.category || '',
          location: searchParams.location || ''
        },
        withCredentials: true
      });

      setEvents(response.data.items || []);
      setTotalPages(response.data.totalPages || 1);
    } catch (error) {
      console.error("Failed to load events:", error);
      setEvents([]);
    }
  };

  useEffect(() => {
    fetchEvents(currentPage, searchParams);
  }, [currentPage, searchParams]);

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

  const handleSearch = (params) => {
    setSearchParams(params);
    setCurrentPage(1);
  };

  return (
    <Box p={4}>

      <EventSearchFilter onSearch={handleSearch} />

      <Box display="flex" flexWrap="wrap" justifyContent="center" gap={4}>
        {events && events.length === 0 ? (
          <Text fontSize="xl" color="gray.500">No events available</Text>
        ) : (
          events.map(event => (
            <EventCard key={event.id} event={event} />
          ))
        )}
      </Box>

      <Box display="flex" justifyContent="center" mt={10}>
        <Button onClick={handlePrevPage} isDisabled={currentPage === 1} mr={2}>
          Previous
        </Button>
        <Text>{currentPage} of {totalPages}</Text>
        <Button onClick={handleNextPage} isDisabled={currentPage === totalPages} ml={2}>
          Next
        </Button>
      </Box>
    </Box>
  );
}

export default EventList;
