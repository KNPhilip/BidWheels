import { Button, ButtonGroup } from 'flowbite-react';
import React from 'react';

type Props = {
  pageSize: number,
  setPageSize: (size: number) => void;
}

const pageSizeButtons = [4, 8, 12];

const Filters = ({pageSize, setPageSize}: Props) =>  {
  return (
    <div className="flex justify-between items-center mb-4">
      <div>
        <span className="uppercase text-sm text-gray-500 mr-2">Page size</span>
        <ButtonGroup>
          {pageSizeButtons.map((value, index) => (
            <Button 
              key={index} 
              onClick={() => setPageSize(value)}
              color={`${pageSize === value ? "red" : "gray"}`}
              className="focus:ring-0"
            >
              {value}
            </Button>
          ))}
        </ButtonGroup>
      </div>
    </div>    
  )
}

export default Filters;