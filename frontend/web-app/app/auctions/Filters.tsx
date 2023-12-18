import { useParamStore } from '@/hooks/useParamsStore';
import { Button } from 'flowbite-react';
import React from 'react';
import { AiOutlineClockCircle, AiOutlineSortAscending } from 'react-icons/ai';
import { BsFillStopCircleFill } from 'react-icons/bs'

const pageSizeButtons = [4, 8, 12];

const orderButtons = [
  {
    label: 'Alphabetical',
    icon: AiOutlineSortAscending,
    value: 'make'
  },
  {
    label: 'End date',
    icon: AiOutlineClockCircle,
    value: 'endingSoon'
  },
  {
    label: 'Recently Added',
    icon: BsFillStopCircleFill,
    value: 'new'
  },
]

const Filters = () =>  {
  const pageSize = useParamStore(state => state.pageSize);
  const setParams = useParamStore(state => state.setParams);
  const orderBy = useParamStore(state => state.orderBy);

  return (
    <div className="flex justify-between items-center mb-4">

      <div>
        <span className="uppercase text-sm text-gray-500 mr-2">Order by</span>
        <Button.Group>
          { orderButtons.map(({label, icon: Icon, value}) => (
            <Button
              key={value}
              onClick={() => setParams({orderBy: value})}
              color={`${orderBy === value ? 'red' : 'gray'}`}
            >
              <Icon className="mr-3 h-4 w-4" />
              {label}
            </Button>
          ))}
        </Button.Group>
      </div>

      <div>
        <span className="uppercase text-sm text-gray-500 mr-2">Page size</span>
        <Button.Group>
          {pageSizeButtons.map((value, index) => (
            <Button 
              key={index} 
              onClick={() => setParams({pageSize: value})}
              color={`${pageSize === value ? "red" : "gray"}`}
              className="focus:ring-0"
            >
              {value}
            </Button>
          ))}
        </Button.Group>
      </div>
    </div>    
  )
}

export default Filters;