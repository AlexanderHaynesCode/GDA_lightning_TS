import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Collapse, Navbar, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { useGlobalContext } from '../GlobalUser';
import MessageIcon from '../Images/MessageImage_blueWhite.png';
import MessageIconNEWNOTIF from '../Images/MessageImage_blueWhite_NEWNOTIF.jpg';
import logo from './GDA_logo3.png';
import './NavMenu.css';

function NavMenu () {
    const { User } = useGlobalContext()
    const [collapsed, setCollapsed] = useState(true);
    const [hasUnreadMessages, setHasUnreadMessages] = useState(false);

    const toggleNavbar = () => {
        setCollapsed(!collapsed);
    };

    useEffect(() => {
        const CheckForUnreadMessages = async () => {
            try {
                const response = await fetch('user/CheckForUnreadMessages/' + User.id);
                const data = await response.json();
                setHasUnreadMessages(data);
            } catch (error) {
                // log error
            }
        };

        if (User.id != 0) {
            CheckForUnreadMessages();
        }        
    }, []);

    if (User.id != 0) {
        return (
            <header>
                <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" container light>
                    <Link to="/">
                        <img className='logo' src={logo} alt="Logo" />
                    </Link>
                    <NavbarToggler onClick={toggleNavbar} className="mr-2" />
                    <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!collapsed} navbar>
                        <ul className="navbar-nav flex-grow">
                            <NavItem className={((User.id == 163) ? 'borderBottom addClickHeightPhone' : 'invisible')}>
                                <NavLink tag={Link} className="text-dark=" to="/admin">
                                    <div className='vert-and-horiz-center black'>ADMIN</div>
                                </NavLink>
                            </NavItem>
                            <NavItem className='borderBottom addClickHeightPhone'>
                                <NavLink tag={Link} className="text-dark=" to="/">
                                    <div className='vert-and-horiz-center black'>Home</div>
                                </NavLink>
                            </NavItem>
                            <NavItem className='borderBottom addClickHeightPhone d67'>
                                <NavLink tag={Link} className="text-dark=" to="/search">
                                    <div className='vert-and-horiz-center black'>Search</div>
                                </NavLink>
                            </NavItem>
                            <NavItem className='borderBottom'>
                                <NavLink tag={Link} className="text-dark" to="/ViewAllConversations">
                                    <div className='vert-and-horiz-center'>
                                        <img className={hasUnreadMessages ? 'invisible' : 'NavBarMessageIcon vertical-center'} src={MessageIcon} alt="Logo" />
                                        <img className={!hasUnreadMessages ? 'invisible' : 'NavBarMessageIcon vertical-center'} src={MessageIconNEWNOTIF} alt="Logo" />
                                    </div>
                                </NavLink>
                            </NavItem>
                            <NavItem>
                                <NavLink tag={Link} className="text-dark addClickHeightPhone d67" to="/Profile">
                                    <div className='vert-and-horiz-center '>Profile</div>
                                </NavLink>
                            </NavItem>
                        </ul>
                    </Collapse>
                </Navbar>
            </header>
        );
    } else {
        return (
            <header>
                <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" container light>
                    <Link to="/">
                        <img className='logo App-logo-spin' src={logo} alt="Logo" />
                    </Link>
                    <NavbarToggler onClick={toggleNavbar} className="mr-2" />
                    <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!collapsed} navbar>
                        <ul className="navbar-nav flex-grow">
                            <NavItem className='borderBottom addClickHeightPhone'>
                                <NavLink tag={Link} className="text-dark=" to="/">
                                    <div className='vert-and-horiz-center black'>Home</div>
                                </NavLink>
                            </NavItem>
                            <NavItem className='borderBottom addClickHeightPhone'>
                                <NavLink tag={Link} className="text-dark=" to="/SignIn">
                                    <div className='vert-and-horiz-center black'>Sign-In</div>
                                </NavLink>
                            </NavItem>
                        </ul>
                    </Collapse>
                </Navbar>
            </header>
        );
    }
            
}

export default NavMenu;


