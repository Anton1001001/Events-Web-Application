import React from 'react';
import { Box, Text, Link, Stack } from '@chakra-ui/react';

const Footer = () => {
  return (
    <Box
      as="footer"
      py={4}
      px={8}
      bg="gray.700"
      color="white"
      position="relative"
      bottom="0"
      width="100%"
      textAlign="center"
    >
      <Stack spacing={4}>
        <Text fontSize="sm">&copy; 2024 Event Management Web Application. All rights reserved.</Text>
        <Stack direction="row" justify="center" spacing={6}>
          <Link href="/events" color="teal.200">Privacy Policy</Link>
          <Link href="/events" color="teal.200">Terms of Service</Link>
          <Link href="/events" color="teal.200">Contact Us</Link>
        </Stack>
      </Stack>
    </Box>
  );
};

export default Footer;
