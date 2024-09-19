import React, { useState, useEffect } from 'react';
import { Input, Select, Box, Button, Stack } from '@chakra-ui/react';

const EventSearchFilter = ({ onSearch }) => {
  const [title, setTitle] = useState('');
  const [date, setDate] = useState('');
  const [category, setCategory] = useState('');
  const [location, setLocation] = useState('');

  const handleSearch = () => {
    onSearch({ title, date, category, location });
  };

  return (
    <Box mb={6}>
      <Stack direction="row" spacing={4}>
        <Input 
            placeholder="Search by Title" 
            value={title} 
            onChange={(e) => setTitle(e.target.value)} 
        />
        <Input 
            type="date" 
            value={date} 
            onChange={(e) => setDate(e.target.value)} 
        />
        <Select placeholder="Select Category" value={category} onChange={(e) => setCategory(e.target.value)}>
            <option value="Вечеринки" >Вечеринки</option>
            <option value="Концерты">Концерты</option>
            <option value = "Форумы">Форумы</option>
            <option value = "Обучение">Обучение</option>
            <option value = "Экскурсии">Экскурсии</option>
        </Select>
        <Select placeholder="Select Location" value={location} onChange={(e) => setLocation(e.target.value)}>
          <option value="Минск">Минск</option>
          <option value="Брест">Брест</option>
          <option value="Гомель">Гомель</option>
          <option value="Гродно">Гродно</option>
          <option value="Могилев">Могилев</option>
          <option value="Витебс">Витебск</option>

        </Select>
        <Button onClick={handleSearch}>Search</Button>
      </Stack>
    </Box>
  );
};

export default EventSearchFilter;
