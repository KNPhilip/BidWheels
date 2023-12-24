'use client'

import { Button } from 'flowbite-react';
import Link from 'next/link';
import React from 'react'

const UserActions = () => {
    return (
        <Button outline>
            <Link href="/session">
                Session
            </Link>
        </Button>
    )
}

export default UserActions;