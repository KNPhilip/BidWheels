import { AiOutlineCar } from 'react-icons/ai'
import Search from './Search';
import Logo from './Logo';

const Navbar = () => {
    return (
        <header className="sticky top-0 z-50 flex justify-between bg-white p-5 items-center text-gray-800 shadow-md">
            <Logo />
            <Search />
            <div>Login</div>
        </header>
    )
}

export default Navbar;