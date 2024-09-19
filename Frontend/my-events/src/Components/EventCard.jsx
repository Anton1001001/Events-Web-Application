import React from 'react';
import { Box, Button, Card, CardBody, CardFooter, Divider, Heading, Image, Stack, Text } from '@chakra-ui/react';
import { Link } from 'react-router-dom';
import config from '../config';

const EventCard = ({ event }) => {
  return (
    <Card height={700} key={event.id} width="300px" m={0}>
      <CardBody>
        <Image
          src={`${config.serverUrl}${event.imageUrl}`}
          alt={event.name}
          borderRadius="lg"
        />
        <Stack mt="6" spacing="3">
          <Heading size="md">{event.name}</Heading>
          <Text color="blue.600" fontSize="2xl">
            {new Date(event.dateTime).toLocaleDateString()} - {event.location}
          </Text>
          <Text 
            color={event.availableSeats > 0 ? "gray.500" : "red.500"} 
            fontSize="sm"
          >
            {event.availableSeats > 0 ? `${event.availableSeats} seats available` : 'No seats available'}
          </Text>
        </Stack>
      </CardBody>
      <Divider />
      <CardFooter>
        <Box textAlign="center" width="full">
          <Link to={`/event/${event.id}`}>
            <Button variant="ghost" colorScheme="blue">
              More Info
            </Button>
          </Link>
        </Box>
      </CardFooter>
    </Card>
  );
};

export default EventCard;
