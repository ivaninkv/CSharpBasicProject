create table users
(
    id     bigint not null
        constraint user_pk
            primary key,
    active boolean
);

alter table users
    owner to postgres;

create table subscription
(
    id                 serial
        constraint subscription_pk
            primary key,
    user_id            bigint
        constraint subscription_users_id_fk
            references users,
    departure_min_date date,
    departure_max_date date
);

alter table subscription
    owner to postgres;

create table origin
(
    subscribe_id integer not null
        constraint origin_subscription_id_fk
            references subscription,
    user_id      bigint  not null,
    iata_code    varchar not null,
    constraint origin_pk
        primary key (subscribe_id, user_id, iata_code)
);

alter table origin
    owner to postgres;

create table destination
(
    subscribe_id integer not null
        constraint destination_subscription_id_fk
            references subscription,
    user_id      bigint  not null,
    iata_code    varchar not null,
    constraint destination_pk
        primary key (subscribe_id, user_id, iata_code)
);

alter table destination
    owner to postgres;
