import React from 'react';
import { Link } from 'react-router-dom';
import { Nav, Navbar, NavItem } from 'reactstrap';
import { LinkContainer } from 'react-router-bootstrap';
import './NavMenu.css';

export default props => (
  <Navbar inverse fixedTop fluid collapseOnSelect>
    <Navbar.Header>
      <Navbar.Brand>
        <Link to={'/'}>Company.WebApplication1</Link>
      </Navbar.Brand>
      <Navbar.Toggle />
    </Navbar.Header>
    <Navbar.Collapse>
      <Nav>
        <LinkContainer to={'/'} exact>
          <NavItem>
            Home
          </NavItem>
        </LinkContainer>
        <LinkContainer to={'/counter'}>
          <NavItem>
            Counter
          </NavItem>
        </LinkContainer>
        <LinkContainer to={'/fetchdata'}>
          <NavItem>
            Fetch data
          </NavItem>
        </LinkContainer>
      </Nav>
    </Navbar.Collapse>
  </Navbar>
);
